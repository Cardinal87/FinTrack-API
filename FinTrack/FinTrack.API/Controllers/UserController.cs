using FinTrack.API.Application.UseCases.Users.CreateUser;
using FinTrack.API.Application.UseCases.Users.DeleteUser;
using FinTrack.API.Application.UseCases.Users.GetUser;
using FinTrack.API.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinTrack.API.Core.Common;
using FinTrack.API.Controllers.Base;

namespace FinTrack.API.Controllers
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UserController : AuthorizeFinTrackControllerBase
    {
        private readonly IMediator _mediator;
        public UserController(IMediator mediator)
        {

            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost()]
        async public Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var command = new CreateUserCommand(request.Phone,
                                                request.Email,
                                                request.Name,
                                                request.Password);

            var result = await _mediator.Send(command);
            if (result.IsSuccess && result.Value != default)
            {
                return CreatedAtAction(nameof(GetUserById),
                                       new { id = result.Value},
                                       new { id = result.Value});
            }
            return HandleFailedResult(result);


        }

        [HttpGet("me")]
        async public Task<IActionResult> GetUserInfo()
        {
            

            var command = new GetUserCommand(UserId);
            var result = await _mediator.Send(command);

                
            if(result.IsSuccess && result.Value != default)
            {
                return Ok(new
                {
                    name = result.Value.Name,
                    phone = result.Value.Phone,
                    email = result.Value.Email,
                });
            }

            return HandleFailedResult(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = Core.Common.UserRoles.Admin)]
        async public Task<IActionResult> GetUserById(Guid id)
        {
            var command = new GetUserCommand(id);
            var result = await _mediator.Send(command);

            if (result.IsSuccess && result.Value != default)
            {
                return Ok(new
                {
                    name = result.Value.Name,
                    phone = result.Value.Phone,
                    email = result.Value.Email,
                    hash = result.Value.PasswordHash
                });
            }

            return HandleFailedResult(result);

        }

        [HttpDelete("me")]
        async public Task<IActionResult> DeleteUser()
        {
            var command = new DeleteUserCommand(UserId);
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
            {
                return NoContent();
            }

            return HandleFailedResult(result);
            
        }

    }

}
