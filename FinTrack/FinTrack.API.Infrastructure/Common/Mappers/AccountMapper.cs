
using AutoMapper;
using FinTrack.API.Core.Entities;
using FinTrack.API.Infrastructure.Common.DTO;

namespace FinTrack.API.Infrastructure.Common.Mappers
{
    public class AccountMapper : Profile
    {
        public AccountMapper()
        {
            CreateMap<Account, AccountDb>()
                .ForMember(t => t.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(t => t.Balance, opt => opt.MapFrom(src => src.Balance))
                .ForMember(t => t.UserId, opt => opt.MapFrom(src => src.UserId));


            CreateMap<AccountDb, Account>()
                .ConstructUsing(src => new Account(src.UserId))
                .ForMember(t => t.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(t => t.OutgoingTransactions, opt => opt.Ignore())
                .ForMember(t => t.Balance, opt => opt.Ignore())
                .ForMember(t => t.IncomingTransactions, opt => opt.Ignore())
                .AfterMap((dbEntity, domainEntity, context) =>
                {
                    if (dbEntity.Balance > 0)
                    {
                        domainEntity.TopUp(dbEntity.Balance);
                    }
                });
        }
    }
}
