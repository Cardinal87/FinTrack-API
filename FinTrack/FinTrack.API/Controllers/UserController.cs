using FinTrack.API.Application.UseCases.Users.CreateUser;
using FinTrack.API.Application.UseCases.Users.DeleteUser;
using FinTrack.API.Application.UseCases.Users.GetUser;
using FinTrack.API.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinTrack.API.Core.Common;
using FinTrack.API.Controllers.Base;
using System.ComponentModel;

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

        /// <summary>
        /// Creates new user
        /// </summary>
        /// <param name="request">data for creating new user</param>
        /// <remarks>
        /// Request example:
        /// POST /api/users
        /// {
        ///     "name": "myname",
        ///     "email": "example@gmail.com",
        ///     "phone": "+79998886655",
        ///     "password": "pwd"
        /// }
        /// 
        /// Response example:
        /// {
        ///     "id": "CREATED_USER_ID"
        /// }
        /// </remarks>
        /// <responce code="201">user created successfully</responce>
        /// <responce code="400">invalid request data</responce>
        [AllowAnonymous]
        [HttpPost()]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Returns info about current user
        /// </summary>
        /// <remarks>
        /// Request example:
        /// GET /api/users/me
        /// -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
        /// 
        /// Response example:
        /// {
        ///     "name": "myname",
        ///     "phone": "+79996668877",
        ///     "email": "exmaple@gmail.com"
        /// }
        /// </remarks>
        /// <responce code="200">successfull request</responce>
        /// <responce code="401">access token is missing or invalid</responce>
        /// <responce code="404">user not found</responce>
        [HttpGet("me")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Returns user info by id
        /// </summary>
        /// <param name="id">Id of the user</param>
        /// <remarks>
        /// Requets example:
        /// GET /api/users/30dd879c-ee2f-11db-8314-0800200c9a66
        /// -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
        /// 
        /// Response example:
        /// {
        ///     "name": "myname",
        ///     "phone": "+79996668877",
        ///     "email": "exmaple@gmail.com"
        ///     "hash": "SHA256.50.Y0ea1poJCyWCd+yPum+ZQZov+ySJgVEGV8lEzNEUjpc=.XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg="
        /// }
        /// </remarks>
        /// <responce code="200">successfull request</responce>
        /// <responce code="401">access token is missing or invalid</responce>
        /// <responce code="403">user does not has access</responce>
        /// <responce code="404">user not found</responce>
        [HttpGet("{id}")]
        [Authorize(Roles = Core.Common.UserRoles.Admin)]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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


        /// <summary>
        /// Deletes current user
        /// </summary>
        /// <remarks>
        /// Request example:
        /// DELETE /api/users/me
        /// -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
        /// </remarks>
        /// <responce code="204">user deleted successfully</responce>
        /// <responce code="401">access token is missing or invalid</responce>
        /// <responce code="404">user not found</responce>
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
