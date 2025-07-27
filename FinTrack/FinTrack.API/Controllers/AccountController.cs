using FinTrack.API.Application.UseCases.Accounts.CreateAccount;
using FinTrack.API.Application.UseCases.Accounts.DebitBalance;
using FinTrack.API.Application.UseCases.Accounts.DeleteAccount;
using FinTrack.API.Application.UseCases.Accounts.DeleteUserAccount;
using FinTrack.API.Application.UseCases.Accounts.GetAccount;
using FinTrack.API.Application.UseCases.Accounts.GetUserAccount;
using FinTrack.API.Application.UseCases.Accounts.TopUpBalance;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace FinTrack.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {

        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost()]
        async public Task<IActionResult> CreateAccount()
        {
            var sub = User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (sub == null)
            {
                return BadRequest(new { message = "jwt token does not contains user id" });
            }
            var guid = Guid.Parse(sub.Value);
            var command = new CreateAccountCommand(guid);
            var accountGuid = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAccountById), new { guid = accountGuid }, new {id = accountGuid});

        }

        [HttpGet("{guid}")]
        async public Task<IActionResult> GetAccountById(Guid guid)
        {
            var command = new GetAccountCommand(guid);
            var account = await _mediator.Send(command);
            if (account == null)
            {
                return BadRequest(new { message = $"account with id {guid} does not exist" });
            }
            return Ok(new
            {
                id = guid,
                balance = account.Balance,
                user_id = account.UserId
            });
        }
        [HttpGet("me/{guid}")]
        async public Task<IActionResult> GetMyAccount(Guid guid)
        {
            var sub = User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (sub == null)
            {
                return BadRequest(new { message = "jwt token does not contains user id" });
            }
            var userGuid = Guid.Parse(sub.Value);
            var command = new GetUserAccountCommand(userGuid, guid);
            var account = await _mediator.Send(command);
            
            if ( account == null)
            {
                return BadRequest(new { message = $"account with id {guid} does not exist" });
            }

            return Ok(new
            {
                id = guid,
                balance = account.Balance,
                user_id = account.UserId
            });
        }



        [HttpDelete("{guid}")]
        async public Task<IActionResult> DeleteAccount(Guid guid)
        {
            var command = new DeleteAccountCommand(guid);
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("me/{guid}")]
        async public Task<IActionResult> DeleteMyAccount(Guid guid)
        {
            var sub = User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (sub == null)
            {
                return BadRequest(new { message = "jwt token does not contains user id" });
            }
            var userId = Guid.Parse(sub.Value);
            var command = new DeleteUserAccountCommand(userId, guid);
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPost("{guid}/topup")]
        async public Task<IActionResult> TopUpAccountBalance(Guid guid, [FromQuery] int amount){
            var command = new TopUpBalanceCommand(guid, amount);
            var balance = await _mediator.Send(command);
            return Ok(new {balance});

        }

        [HttpPost("{guid}/debit")]
        async public Task<IActionResult> DebitAccountBalance(Guid guid, [FromQuery] int amount)
        {
            var command = new DebitBalanceCommand(guid, amount);
            var balance = await _mediator.Send(command);
            return Ok(new { balance });

        }

        
    }
}
