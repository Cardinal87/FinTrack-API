

using AutoMapper;
using FinTrack.API.Core.Entities;
using FinTrack.API.Infrastructure.Data.DbEntities;

namespace FinTrack.API.Infrastructure.Mappers
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<User, UserDb>()
                .ForMember(t => t.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(t => t.Accounts, opt => opt.MapFrom(src => src.Accounts))
                .ForMember(t => t.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(t => t.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(t => t.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(t => t.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash));


            CreateMap<UserDb, User>()
                .ConstructUsing(src => new User(src.Email, src.Phone, src.Name, src.PasswordHash))
                .ForMember(t => t.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(t => t.Accounts, opt => opt.Ignore())
                .AfterMap((dbEntity, domainEntity, context) =>
                {
                    var mapper = context.Mapper;
                    foreach (var accountDb in dbEntity.Accounts)
                    {
                        var account = mapper.Map<Account>(accountDb);
                        domainEntity.AddAccount(account);
                    }
                });
        }
    }
}
