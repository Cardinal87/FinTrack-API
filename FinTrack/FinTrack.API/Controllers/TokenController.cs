using FinTrack.API.Application.Common;
using FinTrack.API.Application.Interfaces;
using FinTrack.API.Application.UseCases.Identity.Commands.RefreshToken;
using FinTrack.API.Application.UseCases.Identity.Commands.RevokeToken;
using FinTrack.API.Application.UseCases.Users.Commands.AuthUser;
using FinTrack.API.Controllers.Base;
using FinTrack.API.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        /// </remarks>
        /// <response code="200">token created</response>
        /// <response code="401">provided credentials are invalid or user does not exists</response>
        [HttpPost("token")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetJwtToken([FromBody] LoginRequest loginRequest, CancellationToken ct)
        {
            var request = new AuthUserCommand(loginRequest.Login, loginRequest.Password);
            var result = await _mediator.Send(request);
            if (result.IsSuccess && result.Value != default)
            {
                return Ok(new { 
                    access_token = result.Value.accessToken,
                    refresh_token = result.Value.refreshToken,
                    expires_in = result.Value.expiresIn
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
            var result = await _mediator.Send(rotateTokenCommand);

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
        [Authorize]
        [HttpPost("token/revoke")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest tokenRequest, CancellationToken ct)
        {
            var command = new RevokeTokenCommand(tokenRequest.RefreshToken);
            await _mediator.Send(command);
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
        [Authorize]
        [HttpGet("token/status")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        public IActionResult GetJwtStatus()
        {
            return Ok(new { status = "token is valid" });
        }


        
    }
}
