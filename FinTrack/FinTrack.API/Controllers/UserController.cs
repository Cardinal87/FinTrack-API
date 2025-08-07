using FinTrack.API.Application.UseCases.Users.CreateUser;
using FinTrack.API.Application.UseCases.Users.DeleteUser;
using FinTrack.API.Application.UseCases.Users.GetUser;
using FinTrack.API.Application.Common;
using FinTrack.API.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using FinTrack.API.Core.Common;

namespace FinTrack.API.Controllers
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
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
            var sub = GetCurrentUserGuid();
            

            var command = new GetUserCommand(sub);
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
        [Authorize(Roles = UserRoles.Admin)]
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
            var sub = GetCurrentUserGuid();
            

            var command = new DeleteUserCommand(sub);
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
            {
                return NoContent();
            }

            return HandleFailedResult(result);
            

            
        }

        private IActionResult HandleFailedResult(ResultBase result)
        {
            switch(result.StatusMessage)
            {
                case OperationStatusMessages.BadRequest: return BadRequest();
                case OperationStatusMessages.Forbidden: return Forbid();
                case OperationStatusMessages.NotFound: return NotFound();
                case OperationStatusMessages.Unauthorized: return Unauthorized();
                default: return StatusCode(500, new { message = "unexpected server error" });
            }
            
        }
        private Guid GetCurrentUserGuid()
        {
            var claim = User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (claim == null) throw new InvalidOperationException("Id claim was not found");
            return Guid.Parse(claim.Value);
        }
    }

}
