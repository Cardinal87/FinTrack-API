
using FinTrack.API.Application.Common;
using FinTrack.API.Core.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Accounts.CreateAccount
{
    
    internal class CreateAccountHandler : IRequestHandler<CreateAccountCommand, ValueResult<Guid>>
    {
        private readonly IAccountRepository _accountRepository;
        
        public CreateAccountHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        
        public async Task<ValueResult<Guid>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var account = new Core.Entities.Account(request.userId);
            _accountRepository.Add(account);
            await _accountRepository.SaveChangesAsync();

            return ValueResult<Guid>.Ok(account.Id, OperationStatusMessages.Created);
        }
    }
}
