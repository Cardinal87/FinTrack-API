using FinTrack.API.Application.Common;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using MediatR;


namespace FinTrack.API.Application.UseCases.Users.Queries.GetUser
{
    internal class GetUserHandler : IRequestHandler<GetUserQuery, ValueResult<User>>
    {
        private readonly IUserRepository _userRepository;

        public GetUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        async public Task<ValueResult<User>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.guid);
            if(user == null)
            {
                return ValueResult<User>.Fail(OperationStatusMessages.NotFound, "user is not found");
            }
            return ValueResult<User>.Ok(user, OperationStatusMessages.Ok);
        }
    }
}
