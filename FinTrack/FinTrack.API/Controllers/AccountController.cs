using FinTrack.API.Application.UseCases.Accounts.CreateAccount;
using FinTrack.API.Application.UseCases.Accounts.DebitBalance;
using FinTrack.API.Application.UseCases.Accounts.DeleteAccount;
using FinTrack.API.Application.UseCases.Accounts.GetAccount;
using FinTrack.API.Application.UseCases.Accounts.TopUpBalance;
using FinTrack.API.Application.UseCases.Users.GetUser;
using FinTrack.API.Core.Common;
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
            var userGuid = GetCurrentUserGuid();
            if (userGuid == null) return BadRequest(new { message = "jwt token does not contains user id" });

            var roles = GetCurrentUserRoles();

            var getAccountCommand = new GetAccountCommand(userGuid.Value, roles, guid);
            var account = await _mediator.Send(getAccountCommand);
            if (account == null) return NotFound(new { message = $"account with id {guid} does not exist" });
            
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
            var userGuid = GetCurrentUserGuid();
            if (userGuid == null) return BadRequest(new { message = "jwt token does not contains user id" });

            var roles = GetCurrentUserRoles();

            var command = new DeleteAccountCommand(userGuid.Value, roles, guid);
            await _mediator.Send(command);
            return NoContent();
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost("{guid}/topup")]
        async public Task<IActionResult> TopUpAccountBalance(Guid guid, [FromQuery] int amount){
            var command = new TopUpBalanceCommand(guid, amount);
            var balance = await _mediator.Send(command);
            return Ok(new {balance});

        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost("{guid}/debit")]
        async public Task<IActionResult> DebitAccountBalance(Guid guid, [FromQuery] int amount)
        {
            var command = new DebitBalanceCommand(guid, amount);
            var balance = await _mediator.Send(command);
            return Ok(new { balance });

        }

        private Guid? GetCurrentUserGuid()
        {
            var claim = User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (claim == null) return null;
            return Guid.Parse(claim.Value);
        }

        private List<string> GetCurrentUserRoles()
        {
            return User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
        }
    }
}
