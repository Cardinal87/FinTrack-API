

using FinTrack.API.Application.Common;
using FinTrack.API.Core.Common;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Users.CreateUser
{
    internal class CreateUserHandler : IRequestHandler<CreateUserCommand, ValueResult<Guid>>
    {
        private IUserRepository _userRepository;
        private IPasswordHasher _passwordHasher;
        
        public CreateUserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }


        
        async public Task<ValueResult<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var hash = _passwordHasher.GetHash(request.password);
            
            var user = new User(request.email,
                                request.phone,
                                request.name,
                                hash);
            user.AssignRole(UserRoles.User);

            _userRepository.Add(user);
            await _userRepository.SaveChangesAsync();
            return ValueResult<Guid>.Ok(user.Id, OperationStatusMessages.Created);
        }
    }
}
