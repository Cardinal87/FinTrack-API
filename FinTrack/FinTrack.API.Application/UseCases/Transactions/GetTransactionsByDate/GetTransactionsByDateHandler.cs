
using FinTrack.API.Application.Common;
using FinTrack.API.Core.Common;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Transactions.GetTransactionsByDate
{
    class GetTransactionsByDateHandler 
        : IRequestHandler<GetTransactionsByDateCommand, ValueResult<IReadOnlyCollection<Transaction>>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;

        public GetTransactionsByDateHandler(ITransactionRepository transactionRepository,
                                            IUserRepository userRepository,
                                            IAccountRepository accountRepository)
        {
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
            _accountRepository = accountRepository;
        }

        async public Task<ValueResult<IReadOnlyCollection<Transaction>>> Handle(GetTransactionsByDateCommand request, CancellationToken cancellationToken)
        {
            if (request.roles.Contains(UserRoles.Admin))
            {
                var transactions = await _transactionRepository.GetByDateAsync(request.date);
                return ValueResult<IReadOnlyCollection<Transaction>>.Ok(transactions.ToList().AsReadOnly(), OperationStatusMessages.Ok);
            }
            var userAccoutsIds = await _accountRepository.GetAccountIdsByUserIdAsync(request.userId);

            var allowed = await _transactionRepository.GetByDateAsync(request.date, userAccoutsIds);
            return ValueResult<IReadOnlyCollection<Transaction>>.Ok(allowed.ToList().AsReadOnly(), OperationStatusMessages.Ok);
        }
    }
}
