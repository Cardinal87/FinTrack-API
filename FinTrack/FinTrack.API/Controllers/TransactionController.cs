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

        /// <summary>
        /// Creates transaction between accounts
        /// </summary>
        /// <param name="request">data for creating transaction</param>
        /// <remarks>
        /// Request example:
        /// POST /api/transactions
        /// -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
        /// {
        ///     "amount": "300",
        ///     "source_account_id": "30dd879c-ee2f-11db-8314-0800200c9a66",
        ///     "destination_account_id": "3b7c138f-fc68-42b8-a705-31417bb4cb56",
        /// }
        /// 
        /// Response example:
        /// {
        ///     "id": "CREATED_Transaction_ID"
        /// }
        /// </remarks>
        /// <responce code="201">transaction created successfully</responce>
        /// <responce code="400">invalid request data</responce>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Returns transaction by id
        /// </summary>
        /// <param name="id">id of the existing transaction</param>
        /// <remarks>
        /// Request example:
        /// GET /api/transactions/30dd879c-ee2f-11db-8314-0800200c9a66
        /// -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
        /// 
        /// Response example:
        /// {
        ///     "id": "30dd879c-ee2f-11db-8314-0800200c9a66",
        ///     "amount": 300,
        ///     "source_account_id": "85cb33aa-f7a5-4940-9bf2-7c50850925aa",
        ///     "destination_account_id": "3b7c138f-fc68-42b8-a705-31417bb4cb56",
        ///     
        /// }
        /// </remarks>
        /// <response code="200">successfull request</response>
        /// <response code="400">invalid request data</response>
        /// <response code="403">user has not access to transaction</response>
        /// <response code="404">transaction with <paramref name="id"/> not found</response>
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        /// <summary>
        /// Returns transaction by date
        /// </summary>
        /// <param name="date">date of the transaction</param>
        /// <remarks>
        /// Request example:
        /// GET /api/transactions/date/2025-08-19
        /// -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
        /// 
        /// Response example:
        /// {
        ///     [    
        ///         {
        ///             "id": "30dd879c-ee2f-11db-8314-0800200c9a66",
        ///             "amount": 300,
        ///             "source_account_id": "85cb33aa-f7a5-4940-9bf2-7c50850925aa",
        ///             "destination_account_id": "3b7c138f-fc68-42b8-a705-31417bb4cb56",
        ///         },
        ///         ...
        ///     ]
        ///     
        /// }
        /// </remarks>
        /// <response code="200">successfull request</response>
        /// <response code="400">invalid request data</response>
        [HttpGet("date/{date}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Returns transaction by time interval
        /// </summary>
        /// <param name="start">start of the interval</param>
        /// <param name="end">end of the interval</param>
        /// <remarks>
        /// Request example:
        /// GET /api/transactions/interval?start=2025-08-10&amp;end=2025-08-13
        /// -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
        /// 
        /// Response example:
        /// {
        ///     [    
        ///         {
        ///             "id": "30dd879c-ee2f-11db-8314-0800200c9a66",
        ///             "amount": 300,
        ///             "source_account_id": "85cb33aa-f7a5-4940-9bf2-7c50850925aa",
        ///             "destination_account_id": "3b7c138f-fc68-42b8-a705-31417bb4cb56",
        ///         },
        ///         ...
        ///     ]
        ///     
        /// }
        /// </remarks>
        /// <response code="200">successfull request</response>
        /// <response code="400">invalid request data</response>
        [HttpGet("interval")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Returns all transactions of specified account
        /// </summary>
        /// <param name="id">id of the existing account</param>
        /// <remarks>
        /// Request example:
        /// GET /api/transactions/account/30dd879c-ee2f-11db-8314-0800200c9a66
        /// -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
        /// 
        /// Response example:
        /// {
        ///     [    
        ///         {
        ///             "id": "85cb33aa-f7a5-4940-9bf2-7c50850925aa",
        ///             "amount": 300,
        ///             "source_account_id": "30dd879c-ee2f-11db-8314-0800200c9a66",
        ///             "destination_account_id": "3b7c138f-fc68-42b8-a705-31417bb4cb56",
        ///         },
        ///         ...
        ///     ]
        ///     
        /// }
        /// </remarks>
        /// <response code="200">successfull request</response>
        /// <response code="400">invalid request data</response>
        /// <response code="403">user has not access to transactions of this account</response>
        /// <response code="404">account with <paramref name="id"/> not found</response>
        [HttpGet("account/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
