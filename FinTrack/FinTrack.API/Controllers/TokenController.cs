using FinTrack.API.Application.Common;
using FinTrack.API.Application.Interfaces;
using FinTrack.API.Application.UseCases.Users.AuthUser;
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
        private readonly IJwtTokenService _jwtTokenService;

        public TokenController(IMediator mediator, IJwtTokenService jwtTokenService)
        {
            _mediator = mediator;
            _jwtTokenService = jwtTokenService;
        }

        /// <summary>
        /// Creates access JWT-token for user
        /// </summary>
        /// <param name="loginRequest">user credentials</param>
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
        ///     "token": YOUR_ACCESS_TOKEN
        /// }
        /// </remarks>
        /// <responce code="200">token created</responce>
        /// <responce code="401">provided credentials are invalid or user does not exists</responce>
        [HttpPost("token")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async public Task<IActionResult> GetJwtToken([FromBody] LoginRequest loginRequest)
        {
            var request = new AuthUserCommand(loginRequest.Login, loginRequest.Password);
            var result = await _mediator.Send(request);
            if (result.IsSuccess && result.Value != default)
            {
                var token = _jwtTokenService.GenerateToken(result.Value);
                return Ok(new { token });
            }
            return HandleFailedResult(result);
        }

        /// <summary>
        /// Returns status of provided JWT-token
        /// </summary>
        /// <remarks>
        /// Request example:
        /// GET /api/auth/token/status
        /// -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
        /// </remarks>
        /// <responce code="200">token is valid</responce>
        /// <responce code="401">token invalid or does not provided</responce>
        [Authorize]
        [HttpGet("token/status")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetJwtStatus()
        {
            return Ok(new { status = "token is valid" });
        }


        
    }
}
