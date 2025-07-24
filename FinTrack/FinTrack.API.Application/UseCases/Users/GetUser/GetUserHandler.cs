using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using MediatR;


namespace FinTrack.API.Application.UseCases.Users.GetUser
{
    internal class GetUserHandler : IRequestHandler<GetUserCommand, User?>
    {
        private readonly IUserRepository _userRepository;

        public GetUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        async public Task<User?> Handle(GetUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.guid);
            return user;
        }
    }
}
