

using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Users.CreateUser
{
    internal class CreateUserHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private IUserRepository _userRepository;
        private IAccountRepository _accountRepository;
        
        
        public CreateUserHandler(IUserRepository userRepository, IAccountRepository accountRepository)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
        }


        /// <summary>
        /// Creates and save User to database
        /// </summary>
        /// <param name="request">DTO with user data</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        async public Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User(request.email,
                                request.phone,
                                request.name,
                                request.hash);

            _userRepository.Add(user);
            await _userRepository.SaveChangesAsync();
            return user.Id;
        }
    }
}
