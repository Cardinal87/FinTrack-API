using FinTrack.API.Application.UseCases.Accounts.CreateAccount;
using FinTrack.API.Application.UseCases.Accounts.DebitBalance;
using FinTrack.API.Application.UseCases.Accounts.DeleteAccount;
using FinTrack.API.Application.UseCases.Accounts.GetAccount;
using FinTrack.API.Application.UseCases.Accounts.TopUpBalance;
using FinTrack.API.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/accounts")]
    public class AccountController : AuthorizeFinTrackControllerBase
    {

        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates account for user
        /// </summary>
        /// <remarks>
        /// Request example:
        /// POST /api/accounts
        /// -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
        /// 
        /// Response example:
        /// {
        ///     "id": "CREATED_ACCOUNT_ID"
        /// }
        /// </remarks>
        /// <response code="201">account created successfully</response>
        [HttpPost()]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        async public Task<IActionResult> CreateAccount()
        {
            var command = new CreateAccountCommand(UserId);
            var result = await _mediator.Send(command);
            if (result.IsSuccess && result.Value != default)
            {
                return CreatedAtAction(nameof(GetAccountById), new { id = result.Value }, new { id = result.Value });
            }
            return HandleFailedResult(result);
        }

        /// <summary>
        /// Returns info abount account by id
        /// </summary>
        /// <param name="id"> id of the existing account</param>
        /// <remarks>
        /// Request example:
        /// GET /api/accounts/30dd879c-ee2f-11db-8314-0800200c9a66
        /// -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
        /// 
        /// Response example:
        /// {
        ///     "id": "30dd879c-ee2f-11db-8314-0800200c9a66",
        ///     "balance": 300,
        ///     "user_id": "3b7c138f-fc68-42b8-a705-31417bb4cb56"
        /// }
        /// </remarks>
        /// <response code="200">successfull request</response>
        /// <response code="400">invalid request data</response>
        /// <response code="403">user has not access to account</response>
        /// <response code="404">account with <paramref name="id"/> not found</response>
        [HttpGet("{id}")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        async public Task<IActionResult> GetAccountById(Guid id)
        {
            var getAccountCommand = new GetAccountCommand(UserId, UserRoles, id);
            var result = await _mediator.Send(getAccountCommand);
            
            if (result.IsSuccess && result.Value != default)
            {
                return Ok(new
                {
                    id = id,
                    balance = result.Value.Balance,
                    user_id = result.Value.UserId
                });
            }
            return HandleFailedResult(result);

            
        }


        /// <summary>
        /// Deletes account by id
        /// </summary>
        /// <param name="id"> id of the existing account</param>
        /// <remarks>
        /// Request example:
        /// DELETE /api/accounts/30dd879c-ee2f-11db-8314-0800200c9a66
        /// -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
        /// 
        /// </remarks>
        /// <response code="204">account delete sucessfully</response>
        /// <response code="400">invalid request data</response>
        /// <response code="403">user has not access to account</response>
        /// <response code="404">account with <paramref name="id"/> not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        async public Task<IActionResult> DeleteAccount(Guid id)
        {
            var command = new DeleteAccountCommand(UserId, UserRoles, id);
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return HandleFailedResult(result);
        }

        /// <summary>
        /// Top ups account balance
        /// </summary>
        /// <param name="id"> id of the existing account</param>
        /// <param name="amount">top up amount</param>
        /// <remarks>
        /// Request example:
        /// POST /api/accounts/30dd879c-ee2f-11db-8314-0800200c9a66/topup?amount=300
        /// -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
        /// 
        /// Response example:
        /// {
        ///     "balance": 300,
        /// }
        /// </remarks>
        /// <response code="204">account topped up successfully</response>
        /// <response code="400">invalid request data</response>
        /// <response code="403">user has not access to endpoint</response>
        /// <response code="404">account with <paramref name="id"/> not found</response>
        [Authorize(Roles = Core.Common.UserRoles.Admin)]
        [HttpPost("{id}/topup")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        async public Task<IActionResult> TopUpAccountBalance(Guid id, [FromQuery] int amount){
            var command = new TopUpBalanceCommand(id, amount);
            var result = await _mediator.Send(command);
            if (result.IsSuccess && result.Value != default)
            {
                return Ok(new { balance = result.Value });
            }
            return HandleFailedResult(result);

        }


        /// <summary>
        /// Debits account balance
        /// </summary>
        /// <param name="id"> id of the existing account</param>
        /// <param name="amount">debit amount</param>
        /// <remarks>
        /// Request example:
        /// POST /api/accounts/30dd879c-ee2f-11db-8314-0800200c9a66/debit?amount=300
        /// -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
        /// 
        /// Response example:
        /// {
        ///     "balance": 0,
        /// }
        /// </remarks>
        /// <response code="204">account debited successfully</response>
        /// <response code="400">invalid request data</response>
        /// <response code="403">user has not access to endpoint</response>
        /// <response code="404">account with <paramref name="id"/> not found</response>
        [Authorize(Roles = Core.Common.UserRoles.Admin)]
        [HttpPost("{id}/debit")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        async public Task<IActionResult> DebitAccountBalance(Guid id, [FromQuery] int amount)
        {
            var command = new DebitBalanceCommand(id, amount);
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(new { balance = result.Value });
            }
            return HandleFailedResult(result);

        }

        
    }
}
