

using AutoMapper;
using FinTrack.API.Core.Entities;
using FinTrack.API.Infrastructure.Common.DTO;

namespace FinTrack.API.Infrastructure.Common.Mappers
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<User, UserDb>()
                .ForMember(t => t.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(t => t.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(t => t.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(t => t.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(t => t.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
                .ForMember(t => t.Roles, opt => opt.MapFrom(src => src.Roles));


            CreateMap<UserDb, User>()
                .ConstructUsing(src => new User(src.Email, src.Phone, src.Name, src.PasswordHash))
                .ForMember(t => t.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(t => t.Accounts, opt => opt.Ignore())
                .ForMember(t => t.Roles, opt => opt.Ignore())
                .AfterMap((dbEntity, domainEntity, context) =>
                {
                    foreach(var role in dbEntity.Roles)
                    {
                        domainEntity.AssignRole(role);
                    }

                });
        }
    }
}
