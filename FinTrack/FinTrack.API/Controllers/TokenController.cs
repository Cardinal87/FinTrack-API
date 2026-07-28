using FinTrack.API.Application.Common;
using FinTrack.API.Application.Interfaces;
using FinTrack.API.Application.UseCases.Identity.Commands.CheckLoginVerificationCode;
using FinTrack.API.Application.UseCases.Identity.Commands.RefreshToken;
using FinTrack.API.Application.UseCases.Identity.Commands.ResendLoginVerificarionCode;
using FinTrack.API.Application.UseCases.Identity.Commands.RevokeToken;
using FinTrack.API.Application.UseCases.Users.Commands.AuthUser;
using FinTrack.API.Controllers.Base;
using FinTrack.API.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FinTrack.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class TokenController : FinTrackContollerBase
    {
        private readonly IMediator _mediator;
        public TokenController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates access-refresh token pair for user by credentials
        /// or sends verification code to user email and creates challenge token if 2fa enabled
        /// </summary>
        /// <param name="loginRequest">user credentials</param>
        /// <param name="ct">cancellation token</param> 
        /// <remarks>
        /// Request example:
        /// POST /api/auth/token
        /// {
        ///     "login": "mylogin",
        ///     "password": "mypassword"
        /// }
        /// 
        /// Response example:
        /// {
        ///     "access_token": "eyJ...",
        ///     "refresh_token": "...",
        ///     "expires_in": 900
        /// }
        /// 
        /// OR
        /// 
        /// Response example:
        /// {
        ///     "message": "verific ation code was sent to your email"
        ///     "challenge_token": "eyJ...",
        ///     "expires_in": 90,
        ///     "_links": {
        ///          "verify": {
        ///             "href": "/api/auth/2fa/complete",
        ///             "method": "POST",
        ///             "title": "verification code confirmation"
        ///          },
        ///          "resend": {
        ///             "href": "/api/auth/2fa/resend",
        ///             "method": "POST",
        ///             "title": "resend verification code"
        ///          }
        ///     }
        /// }
        /// </remarks>
        /// <response code="200">access token created</response>
        /// <response code="202">challenge token created and code was sent to email</response>
        /// <response code="401">provided credentials are invalid or user does not exists</response>
        /// <response code="403">provided credentials are valid, 2fa enabled but email is not verified yet</response>
        [HttpPost("token")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetJwtToken([FromBody] LoginRequest loginRequest, CancellationToken ct)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "undefined";
            var userAgent = HttpContext.Request.Headers.UserAgent.ToString();

            var request = new AuthUserCommand(loginRequest.Login, loginRequest.Password, ip, userAgent);
            var result = await _mediator.Send(request, ct);

            if (result.StatusMessage == OperationStatusMessages.Ok && result.Value != default)
            {
                return Ok(new { 
                    access_token = result.Value.token,
                    refresh_token = result.Value.refreshToken,
                    expires_in = result.Value.expiresIn
                });
            }

            if (result.StatusMessage == OperationStatusMessages.Accepted && result.Value != default)
            {
                return Accepted(new
                {
                    message = "verifivation code was sent to your email",
                    challenge_token = result.Value.token,
                    expires_in = result.Value.expiresIn,
                    _links = new Dictionary<string, object>
                    {
                        {
                            "verify", new
                            {
                                href = "/api/auth/2fa/complete",
                                method = "POST",
                                title = "verification code confirmation"
                            }
                        },
                        {
                            "resend", new
                            {
                                href = "/api/auth/2fa/resend",
                                method = "POST",
                                title = "resend verification code"
                            }
                        }
                    }
                });
            }
            return HandleFailedResult(result);
        }

        /// <summary>
        /// Completes authentication by verification otp code 
        /// </summary>
        /// <param name="request">Verification code</param>
        /// <param name="ct">Cancellation token</param>
        /// <remarks>
        /// Request example:
        /// POST /api/auth/2fa/complete
        /// -H "Authorization: Bearer YOUR_CHALLENGE_TOKEN"
        /// {
        ///     "code": 123456
        /// }
        /// 
        /// Response example:
        /// {
        ///     "access_token": "eyJ...",
        ///     "refresh_token": "...",
        ///     "expires_in": 900
        /// }
        /// </remarks>
        /// <response code="200">access token created</response>
        /// <response code="401">provided code is invalid or user does not exists</response>
        [HttpPost("2fa/complete")]
        [Authorize(Policy = "MfaPending")]
        [EnableRateLimiting("MfaCompleteLimit")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> CompleteLogin([FromBody] VerifyCodeRequest request, CancellationToken ct)
        {
            var userId = GetCurrentUserGuid();
            var jti = GetJti();

            var command = new CheckLoginVerificationCodeCommand(userId, jti, request.Code);
            var result = await _mediator.Send(command, ct);

            if (result.IsSuccess && result.Value != default)
            {
                return Ok(new
                {
                    access_token = result.Value.token,
                    refresh_token = result.Value.refreshToken,
                    expires_in = result.Value.expiresIn
                });
            }
            return HandleFailedResult(result);
        }


        /// <summary>
        /// Sends new verification code to user email
        /// </summary>
        /// <param name="ct">cancellation token</param>
        /// <remarks>
        /// Request example:
        /// POST /api/auth/2fa/resend
        /// -H "Authorization: Bearer YOUR_CHALLENGE_TOKEN"
        /// 
        /// Response example:
        /// {
        ///     "message": "verification code was sent to your email"
        ///     "challenge_token": "eyJ...",
        ///     "expires_in": 90,
        ///     "_links": {
        ///          "verify": {
        ///             "href": "/api/auth/2fa/complete",
        ///             "method": "POST",
        ///             "title": "verification code confirmation"
        ///          },
        ///          "resend": {
        ///             "href": "/api/auth/2fa/resend",
        ///             "method": "POST",
        ///             "title": "resend verification code"
        ///          }
        ///     }
        /// }
        /// </remarks>
        /// <response code="202">challenge token created and code was sent to email</response>
        /// <response code="401">provided credentials are invalid or user does not exists</response>
        [HttpPost("2fa/resend")]
        [Authorize(Policy = "MfaPending")]
        [Produces("application/json")]
        [EnableRateLimiting("MfaResendCodeLimit")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> ResendCode(CancellationToken ct)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "undefined";
            var userAgent = HttpContext.Request.Headers.UserAgent.ToString();

            var userId = GetCurrentUserGuid();
            var jti = GetJti();

            var command = new ResendLoginVerificationCodeCommand(userId, jti, userAgent, ip);
            var result = await _mediator.Send(command, ct);

            if (result.IsSuccess && result.Value != default)
            {
                return Accepted(new
                {
                    message = "verifivation code was sent to your email",
                    challenge_token = result.Value.token,
                    expires_in = result.Value.expiresIn,
                    _links = new Dictionary<string, object>
                    {
                        {
                            "verify", new
                            {
                                href = "/api/auth/2fa/complete",
                                method = "POST",
                                title = "verification code confirmation"
                            }
                        },
                        {
                            "resend", new
                            {
                                href = "/api/auth/2fa/resend",
                                method = "POST",
                                title = "resend verification code"
                            }
                        }
                    }
                });
            }
            return HandleFailedResult(result);
        }


        /// <summary>
        /// Creates access-refresh token pair for user by refresh token
        /// </summary>
        /// <param name="tokenRequest">valid refresh token</param>
        /// <param name="ct">cancellation token</param> 
        /// <remarks>
        /// Request example:
        /// POST /api/auth/token/refresh
        /// {
        ///     "refresh_token": "..." 
        /// }
        /// 
        /// Response example:
        /// {
        ///     "access_token": "eyJ...",
        ///     "refresh_token": "...",
        ///     "expires_in": 900
        /// }
        /// </remarks>
        /// <response code="200">token created</response>
        /// <response code="401">provided credentials are invalid or user does not exists</response>
        [HttpPost("token/refresh")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest tokenRequest, CancellationToken ct)
        {
            var rotateTokenCommand = new RefreshTokenCommand(tokenRequest.RefreshToken);
            var result = await _mediator.Send(rotateTokenCommand, ct);

            if (result.IsSuccess && result.Value != default)
            {
                return Ok(new
                {
                    access_token = result.Value.token,
                    refresh_token = result.Value.refreshToken,
                    expires_in = result.Value.expiresIn,
                });
            }
            return HandleFailedResult(result);
        }


        /// <summary>
        /// Revoked a refresh token.
        /// Requires valid access token
        /// </summary>
        /// <param name="tokenRequest">valid refresh token</param>
        /// <param name="ct">cancellation token</param>
        /// <remarks>
        /// Request example:
        /// POST /api/auth/token/revoke
        /// -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
        /// {
        ///     "refresh_token": "YOUR_REFRESH_TOKEN"
        /// }
        /// </remarks>
        /// <response code="204">token revoked</response>
        /// <response code="401">access or refresh token invalid or does not provided</response>
        [Authorize(Policy = "AccessToken")]
        [HttpPost("token/revoke")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest tokenRequest, CancellationToken ct)
        {
            var command = new RevokeTokenCommand(tokenRequest.RefreshToken);
            await _mediator.Send(command, ct);
            return NoContent();
        }

        /// <summary>
        /// Returns status of provided JWT-token
        /// </summary>
        /// <remarks>
        /// Request example:
        /// GET /api/auth/token/status
        /// -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
        /// </remarks>
        /// <response code="200">access token is valid</response>
        /// <response code="401">access token invalid or does not provided</response>
        [Authorize(Policy = "AccessToken")]
        [HttpGet("token/status")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        public IActionResult GetJwtStatus()
        {
            return Ok(new { status = "token is valid" });
        }


        private Guid GetCurrentUserGuid()
        {
            var claim = User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (claim == null) throw new InvalidOperationException("Id claim was not found");
            return Guid.Parse(claim.Value);
        }
        private Guid GetJti()
        {
            var jti = User.FindFirst(t => t.Type == JwtRegisteredClaimNames.Jti)?.Value;
            if (jti == null) throw new InvalidOperationException("Jti claim was not found");
            return Guid.Parse(jti);
        }
    }
}
