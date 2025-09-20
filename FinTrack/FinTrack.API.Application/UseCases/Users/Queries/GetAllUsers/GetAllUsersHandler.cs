using FinTrack.API.Application.Common;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Users.Queries.GetAllUsers
{
    internal class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, ValueResult<IReadOnlyCollection<User>>>
    {
        private readonly IUserRepository _userRepository;

        public GetAllUsersHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        async public Task<ValueResult<IReadOnlyCollection<User>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync(request.pageNumber, request.pageSize);
            return ValueResult<IReadOnlyCollection<User>>.Ok(users.ToList().AsReadOnly(), OperationStatusMessages.Ok);
        }
    }
}
