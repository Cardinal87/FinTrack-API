
using AutoMapper;
using FinTrack.API.Core.Entities;
using FinTrack.API.Infrastructure.Data.DbEntities;
namespace FinTrack.API.Infrastructure.Mappers
{
    public class AccountMapper : Profile
    {
        public AccountMapper()
        {
            CreateMap<Account, AccountDb>()
                .ForMember(t => t.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(t => t.Balance, opt => opt.MapFrom(src => src.Balance))
                .ForMember(t => t.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(t => t.OutgoingTransactions, opt => opt.MapFrom(src => src.OutgoingTransactions))
                .ForMember(t => t.IncomingTransactions, opt => opt.MapFrom(src => src.IncomingTransactions));


            CreateMap<AccountDb, Account>()
                .ConstructUsing(src => new Account(src.UserId))
                .ForMember(t => t.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(t => t.OutgoingTransactions, opt => opt.Ignore())
                .ForMember(t => t.Balance, opt => opt.Ignore())
                .ForMember(t => t.IncomingTransactions, opt => opt.Ignore())
                .AfterMap((dbEntity, domainEntity, context) =>
                {
                    var mapper = context.Mapper;
                    if (domainEntity.Balance > 0)
                    {
                        domainEntity.TopUp(dbEntity.Balance);
                    }
                    foreach(var outgoing in dbEntity.OutgoingTransactions)
                    {
                        var transaction = mapper.Map<Transaction>(outgoing);
                        domainEntity.AddOutgoingTransaction(transaction);
                    }
                    foreach(var incoming in dbEntity.IncomingTransactions)
                    {
                        var transaction = mapper.Map<Transaction>(incoming);
                        domainEntity.AddIncomingTransaction(transaction);
                    }
                });
        }
    }
}
