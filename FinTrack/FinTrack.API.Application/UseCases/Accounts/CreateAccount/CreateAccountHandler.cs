
using FinTrack.API.Core.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Accounts.CreateAccount
{
    /// <summary>
    /// Handles account creating 
    /// </summary>
    /// <remarks>
    /// Steps:
    /// <para>1. Create account with given user id</para>
    /// <para>2. Add account to repository and save changes</para>
    /// <para>3. Return id of the created account</para>
    /// 
    /// </remarks>
    internal class CreateAccountHandler : IRequestHandler<CreateAccountCommand, Guid>
    {
        private readonly IAccountRepository _accountRepository;
        
        public CreateAccountHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        
        public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var account = new Core.Entities.Account(request.userId);
            _accountRepository.Add(account);
            await _accountRepository.SaveChangesAsync();

            return account.Id;
        }
    }
}
