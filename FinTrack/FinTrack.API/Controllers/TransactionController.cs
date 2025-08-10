using FinTrack.API.Application.Common;
using FinTrack.API.Application.UseCases.Transactions.CreateTransaction;
using FinTrack.API.Application.UseCases.Transactions.GetAccountTransactions;
using FinTrack.API.Application.UseCases.Transactions.GetTransactionById;
using FinTrack.API.Application.UseCases.Transactions.GetTransactionsByDate;
using FinTrack.API.Application.UseCases.Transactions.GetTransactionsByTimeInterval;
using FinTrack.API.Controllers.Base;
using FinTrack.API.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FinTrack.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/transactions")]
    public class TransactionController : AuthorizeFinTrackControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        async public Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest request)
        {
            var createTransactionCommand = new CreateTransactionCommand(request.Amount,
                                                                        request.SourceAccountId,
                                                                        request.DestinationAccountId);
            var result = await _mediator.Send(createTransactionCommand);

            if (result.IsSuccess && result.Value != default)
            {
                return CreatedAtAction(nameof(GetTransactionById),
                                       new { guid = result.Value },
                                       new { guid = result.Value });
            }
            return HandleFailedResult(result);
        }


        [HttpGet("{id}")]
        async public Task<IActionResult> GetTransactionById(Guid id)
        {
            

            var getTransactionsCommand = new GetTransactionByIdCommand(UserId, UserRoles, id);
            var result = await _mediator.Send(getTransactionsCommand);

            if (result.IsSuccess && result.Value != default)
            {
                return Ok(new
                {
                    id = result.Value.Id,
                    source_account_id = result.Value.FromAccountId,
                    destination_account_id = result.Value.ToAccountId,
                    amount = result.Value.Amount,
                });
            }
            return HandleFailedResult(result);

        }

        [HttpGet("date/{date}")]
        async public Task<IActionResult> GetTransactionsByDate(DateOnly date)
        {

            var getTransactionByDateCommand = new GetTransactionsByDateCommand(UserId, UserRoles, date);
            var result = await _mediator.Send(getTransactionByDateCommand);

            if (result.IsSuccess && result.Value != default)
            {
                return Ok(new
                {
                    transactions = result.Value
                });
            }
            return HandleFailedResult(result);
        }

        [HttpGet("interval")]
        async public Task<IActionResult> GetTransactionsByInterval([FromQuery] DateTime start,
                                                                   [FromQuery] DateTime end)
        {

            var getTransactionByIntervalCommand = new GetTransactionsByTimeIntervalCommand(UserId,
                                                                                           UserRoles,
                                                                                           start,
                                                                                           end);
            var result = await _mediator.Send(getTransactionByIntervalCommand);
            if (result.IsSuccess && result.Value != default)
            {
                return Ok(new
                {
                    transactions = result.Value
                });
            }
            return HandleFailedResult(result);
        }

        [HttpGet("account/{id}")]
        async public Task<IActionResult> GetAccountTransactions(Guid id)
        {

            var getAccountTransactionCommand = new GetAccountTransactionsCommand(UserId, UserRoles, id);
            var result = await _mediator.Send(getAccountTransactionCommand);

            if (result.IsSuccess && result.Value != default)
            {
                return Ok(new
                {
                    transactions = result.Value
                });
            }
            return HandleFailedResult(result);
        }

        

    }
}
