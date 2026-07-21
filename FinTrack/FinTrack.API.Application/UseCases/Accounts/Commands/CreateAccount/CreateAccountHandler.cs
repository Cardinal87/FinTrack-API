using FinTrack.API.Application.Common;
using FinTrack.API.Core.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Accounts.Commands.CreateAccount
{
    
    internal class CreateAccountHandler : IRequestHandler<CreateAccountCommand, ValueResult<Guid>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;
        
        public CreateAccountHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<ValueResult<Guid>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var account = new Core.Entities.Account(request.userId);
            await _accountRepository.AddAsync(account);
            await _unitOfWork.SaveChangesAsync();

            return ValueResult<Guid>.Ok(account.Id, OperationStatusMessages.Created);
        }
    }
}
