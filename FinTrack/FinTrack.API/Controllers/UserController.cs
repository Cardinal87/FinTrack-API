using FinTrack.API.Application.UseCases.Users.CreateUser;
using FinTrack.API.Application.UseCases.Users.DeleteUser;
using FinTrack.API.Application.UseCases.Users.GetUser;
using FinTrack.API.Core.Interfaces;
using FinTrack.API.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace FinTrack.API.Controllers
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserRepository _userRepository;
        public UserController(IMediator mediator, IUserRepository userRepository)
        {
            _mediator = mediator;
            _userRepository = userRepository;
        }

        [AllowAnonymous]
        [HttpPost()]
        async public Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var command = new CreateUserCommand(request.Phone,
                                                request.Email,
                                                request.Name,
                                                request.Password);

            var guid = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetUserById), new { id = guid }, new { id = guid });
        }

        [HttpGet("me")]
        async public Task<IActionResult> GetUserInfo()
        {
            var sub = User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (sub != null)
            {
                var guid = Guid.Parse(sub.Value);
                var command = new GetUserCommand(guid);
                var user = await _mediator.Send(command);


                if (user == null) return BadRequest(new { message = $"user with guid {guid} does not exits" });

                return Ok(new
                {
                    name = user.Name,
                    phone = user.Phone,
                    email = user.Email,
                });
            }

            return BadRequest(new { message = "token does not contatins user id" });
        }

        [HttpGet("{guid}")]
        async public Task<IActionResult> GetUserById(Guid guid)
        {
            var command = new GetUserCommand(guid);
            var user = await _mediator.Send(command);

            if (user == null) return BadRequest(new { message = $"user with guid {guid} does not exits" });

            return Ok(new
            {
                name = user.Name,
                phone = user.Phone,
                email = user.Email,
                hash = user.PasswordHash
            });
        }

        [HttpDelete("me")]
        async public Task<IActionResult> DeleteUser()
        {
            var sub = User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (sub != null)
            {
                var guid = Guid.Parse(sub.Value);
                var command = new DeleteUserCommand(guid);
                await _mediator.Send(command);
                return NoContent();
            }

            return BadRequest(new { message = "token does not contatins user id" });
        }


    }

}
