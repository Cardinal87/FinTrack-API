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

        [HttpPost("token")]
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


        [Authorize]
        [HttpGet("token/status")]
        public IActionResult GetJwtStatus()
        {
            return Ok(new { status = "token is valid" });
        }


        
    }
}
