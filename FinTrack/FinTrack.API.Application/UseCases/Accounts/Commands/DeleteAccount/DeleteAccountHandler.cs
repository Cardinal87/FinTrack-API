

using FinTrack.API.Core.Interfaces;
using FinTrack.API.Core.Common;
using MediatR;
using FinTrack.API.Application.Common;

namespace FinTrack.API.Application.UseCases.Accounts.Commands.DeleteAccount
{
    
    internal class DeleteAccountHandler : IRequestHandler<DeleteAccountCommand, Result>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAccountHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
        }

        async public Task<Result> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.accountId);
            if(account == null)
            {
                return Result.Fail(OperationStatusMessages.NotFound, "account was not found");
            }
            if (request.roles.Contains(UserRoles.Admin) ||
                request.userId == account.UserId)
            {
                await _accountRepository.DeleteAsync(request.accountId);
                await _unitOfWork.SaveChangesAsync();
                return Result.Ok(OperationStatusMessages.NoContent);
            }
            return Result.Fail(OperationStatusMessages.Forbidden, "you do not have permission to delete this account.");


        }
    }
}
