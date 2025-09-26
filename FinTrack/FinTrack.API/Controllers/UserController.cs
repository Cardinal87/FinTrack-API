using FinTrack.API.Application.UseCases.Users.Commands.CreateUser;
using FinTrack.API.Application.UseCases.Users.Commands.DeleteUser;
using FinTrack.API.Application.UseCases.Users.Queries.GetUser;
using FinTrack.API.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinTrack.API.Controllers.Base;
using FinTrack.API.Application.UseCases.Users.Queries.GetAllUsers;
using FinTrack.API.Application.UseCases.Users.Commands.UpdateUser;

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
        /// <response code="201">user created successfully</response>
        /// <response code="400">invalid request data</response>
        [AllowAnonymous]
        [HttpPost()]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
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
        /// Updates current user
        /// </summary>
        /// <param name="request">data for updating user</param>
        /// <remarks>
        /// Requets example:
        /// PUT /api/users/me
        /// -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
        /// {
        ///     "name": "new_name",
        ///     "email": "new_example@gmail.com",
        ///     "phone": "+79998886655"
        /// }
        /// 
        /// </remarks>
        /// <response code="204">successfull request</response>
        /// <response code="400">invalid request data</response>
        /// <response code="401">access token is missing or invalid</response>
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [HttpPut("me")]
        async public Task<IActionResult> UpdateMe([FromBody] UpdateUserRequest request)
        {
            var command = new UpdateUserCommand(request.Name, request.Email, request.Phone, UserId);
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return HandleFailedResult(result);
        }

        /// <summary>
        /// Updated user by id
        /// </summary>
        /// <param name="request">data for updating user</param>
        /// <param name="id">Id of the user that will be updated</param>
        /// <remarks>
        /// Requets example:
        /// PUT /api/users/30dd879c-ee2f-11db-8314-0800200c9a66
        /// -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
        /// {
        ///     "name": "new_name",
        ///     "email": "new_example@gmail.com",
        ///     "phone": "+79998886655"
        /// }
        /// 
        /// </remarks>
        /// <response code="204">successfull request</response>
        /// <response code="400">invalid request data</response>
        /// <response code="401">access token is missing or invalid</response>
        /// <response code="403">user does not has access</response>
        /// <response code="404">user with <paramref name="id"/> not found</response>
        [Authorize(Roles = Core.Common.UserRoles.Admin)]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [HttpPut("{id}")]
        async public Task<IActionResult> UpdateUserById([FromBody] UpdateUserRequest request, Guid id)
        {
            var command = new UpdateUserCommand(request.Name, request.Email, request.Phone, id);
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
            {
                return NoContent();
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
        /// <response code="200">successfull request</response>
        /// <response code="401">access token is missing or invalid</response>
        /// <response code="404">user not found</response>
        [HttpGet("me")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        async public Task<IActionResult> GetUserInfo()
        {
            var command = new GetUserQuery(UserId);
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
        /// <response code="200">successfull request</response>
        /// <response code="401">access token is missing or invalid</response>
        /// <response code="403">user does not has access</response>
        /// <response code="404">user not found</response>
        [HttpGet("{id}")]
        [Authorize(Roles = Core.Common.UserRoles.Admin)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        async public Task<IActionResult> GetUserById(Guid id)
        {
            var command = new GetUserQuery(id);
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
        /// Returns all users
        /// </summary>
        /// <param name="page_num">page number of the paginated result (default: 1)</param>
        /// <param name="page_size">page size of the paginated result, maximum size is 1000 (default: 50)</param>
        /// <remarks>
        /// Requets example:
        /// GET /api/users
        /// -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
        /// 
        /// Response example:
        /// {
        ///     "users": [    
        ///                 {
        ///                     "id": "30dd879c-ee2f-11db-8314-0800200c9a66",
        ///                     "name": "myname",
        ///                     "phone": "+79996668877",
        ///                     "email": "exmaple@gmail.com"
        ///                     "hash": "SHA256.50.Y0ea1poJCyWCd+yPum+ZQZov+ySJgVEGV8lEzNEUjpc=.XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg="
        ///                 },
        ///                 ...
        ///              ]
        ///     
        /// }
        /// </remarks>
        /// <response code="200">successfull request</response>
        /// <response code="401">access token is missing or invalid</response>
        /// <response code="403">user does not has access</response>
        [HttpGet()]
        [Authorize(Roles = Core.Common.UserRoles.Admin)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ProblemDetails))]
        async public Task<IActionResult> GetAllUsers([FromQuery] int page_num = 1, [FromQuery] int page_size = 50)
        {
            var command = new GetAllUsersQuery(page_num, page_size);
            var result = await _mediator.Send(command);

            if (result.IsSuccess && result.Value != default)
            {
                return Ok(new
                {
                    users = result.Value.Select(x => new { x.Id, x.Name, x.Phone, x.Email, x.PasswordHash })
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
        /// <response code="204">user deleted successfully</response>
        /// <response code="401">access token is missing or invalid</response>
        /// <response code="404">user not found</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
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

        /// <summary>
        /// Deletes user by id
        /// </summary>
        /// <remarks>
        /// Request example:
        /// DELETE /api/users/30dd879c-ee2f-11db-8314-0800200c9a66
        /// -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
        /// </remarks>
        /// <response code="204">user deleted successfully</response>
        /// <response code="401">access token is missing or invalid</response>
        /// <response code="403">user does not has access</response>
        /// <response code="404">user not found</response>
        [Authorize(Roles = Core.Common.UserRoles.Admin)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [HttpDelete("{id}")]
        async public Task<IActionResult> DeleteUserById(Guid id)
        {
            var command = new DeleteUserCommand(id);
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
            {
                return NoContent();
            }

            return HandleFailedResult(result);
        }

    }

}
