using FinTrack.API.Application.Common;
using FinTrack.API.Application.UseCases.Accounts.CreateAccount;
using FinTrack.API.Application.UseCases.Accounts.DebitBalance;
using FinTrack.API.Application.UseCases.Accounts.DeleteAccount;
using FinTrack.API.Application.UseCases.Accounts.GetAccount;
using FinTrack.API.Application.UseCases.Accounts.TopUpBalance;
using FinTrack.API.Application.UseCases.Users.GetUser;
using FinTrack.API.Controllers.Base;
using FinTrack.API.Core.Common;
using FinTrack.API.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Data;
using System.Security.Claims;

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

        [HttpPost()]
        async public Task<IActionResult> CreateAccount()
        {
            var command = new CreateAccountCommand(UserId);
            var result = await _mediator.Send(command);
            if (result.IsSuccess && result.Value != default)
            {
                return CreatedAtAction(nameof(GetAccountById), new { guid = result.Value }, new { id = result.Value });
            }
            return HandleFailedResult(result);
        }

        [HttpGet("{guid}")]
        async public Task<IActionResult> GetAccountById(Guid guid)
        {
            var getAccountCommand = new GetAccountCommand(UserId, UserRoles, guid);
            var result = await _mediator.Send(getAccountCommand);
            
            if (result.IsSuccess && result.Value != default)
            {
                return Ok(new
                {
                    id = guid,
                    balance = result.Value.Balance,
                    user_id = result.Value.UserId
                });
            }
            return HandleFailedResult(result);

            
        }
       


        [HttpDelete("{guid}")]
        async public Task<IActionResult> DeleteAccount(Guid guid)
        {
            var command = new DeleteAccountCommand(UserId, UserRoles, guid);
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return HandleFailedResult(result);
        }


        [Authorize(Roles = Core.Common.UserRoles.Admin)]
        [HttpPost("{guid}/topup")]
        async public Task<IActionResult> TopUpAccountBalance(Guid guid, [FromQuery] int amount){
            var command = new TopUpBalanceCommand(guid, amount);
            var result = await _mediator.Send(command);
            if (result.IsSuccess && result.Value != default)
            {
                return Ok(new { balance = result.Value });
            }
            return HandleFailedResult(result);

        }

        [Authorize(Roles = Core.Common.UserRoles.Admin)]
        [HttpPost("{guid}/debit")]
        async public Task<IActionResult> DebitAccountBalance(Guid guid, [FromQuery] int amount)
        {
            var command = new DebitBalanceCommand(guid, amount);
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(new { balance = result.Value });
            }
            return HandleFailedResult(result);

        }

        
    }
}
