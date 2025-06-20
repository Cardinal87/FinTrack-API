using AutoMapper;
using FinTrack.API.Infrastructure.Data.DbEntities;
using FinTrack.API.Core.Entities;

namespace FinTrack.API.Infrastructure.Mappers
{
    public class TransactionMapper : Profile
    {
        public TransactionMapper()
        {
            CreateMap<Transaction, TransactionDb>()
                .ForMember(t => t.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(t => t.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(t => t.FromAccountId, opt => opt.MapFrom(src => src.FromAccountId))
                .ForMember(t => t.FromAccount, opt => opt.Ignore())
                .ForMember(t => t.ToAccountId, opt => opt.MapFrom(src => src.ToAccountId))
                .ForMember(t => t.ToAccount, opt => opt.Ignore())
                .ForMember(t => t.Date, opt => opt.MapFrom(src => src.Date));

            CreateMap<TransactionDb, Transaction>()
                .ConstructUsing(src => new Transaction(src.Amount, src.FromAccountId, src.ToAccountId, src.Date))
                .ForMember(t => t.Id, opt => opt.MapFrom(src => src.Id));
        }
        
    }
}
