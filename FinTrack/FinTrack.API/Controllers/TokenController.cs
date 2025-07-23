using FinTrack.API.Application.Interfaces;
using FinTrack.API.Application.UseCases.Users.AuthUser;
using FinTrack.API.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class TokenController : ControllerBase
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
            var authUser = await _mediator.Send(request);
            if (authUser == null)
            {
                return Unauthorized(new { message = "invalid credentials" });
            }
            var token = _jwtTokenService.GenerateToken(authUser, new List<string> { "User" });
            return Ok(new {token});
        }


        [Authorize]
        [HttpGet("token/status")]
        public IActionResult GetJwtStatus()
        {
            return Ok(new { status = "token is valid" });
        }
    }
}
