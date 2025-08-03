
using FinTrack.API.Application.Common;
using FinTrack.API.Core.Common;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Transactions.GetTransactionById
{
    internal class GetTransactionByIdHandler : IRequestHandler<GetTransactionByIdCommand, ValueResult<Transaction>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;

        public GetTransactionByIdHandler(ITransactionRepository transactionRepository, IAccountRepository accountRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
        }

        async public Task<ValueResult<Transaction>> Handle(GetTransactionByIdCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _transactionRepository.GetByIdAsync(request.transactionId);
            if (transaction == null)
            {
                return ValueResult<Transaction>.Fail(OperationStatusMessages.NotFound);
            }
            if (request.roles.Contains(UserRoles.Admin))
            {
                return ValueResult<Transaction>.Ok(transaction, OperationStatusMessages.Ok);
            }

            var fromAccount = await _accountRepository.GetByIdAsync(transaction.FromAccountId);
            var toAccount = await _accountRepository.GetByIdAsync(transaction.ToAccountId);

            if (fromAccount != null && toAccount != null && 
                (toAccount.UserId == request.userId || fromAccount.UserId == request.userId))
            {
                return ValueResult<Transaction>.Ok(transaction, OperationStatusMessages.Ok);
            }
            return ValueResult<Transaction>.Fail(OperationStatusMessages.Forbidden);
        }
    }
}
