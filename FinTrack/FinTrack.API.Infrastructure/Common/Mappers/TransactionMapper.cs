using AutoMapper;
using FinTrack.API.Core.Entities;
using FinTrack.API.Infrastructure.Common.DTO;

namespace FinTrack.API.Infrastructure.Common.Mappers
{
    public class TransactionMapper : Profile
    {
        public TransactionMapper()
        {
            CreateMap<Transaction, TransactionDb>()
                .ForMember(t => t.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(t => t.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(t => t.FromAccountId, opt => opt.MapFrom(src => src.FromAccountId))
                .ForMember(t => t.ToAccountId, opt => opt.MapFrom(src => src.ToAccountId))
                .ForMember(t => t.Date, opt => opt.MapFrom(src => src.Date));

            CreateMap<TransactionDb, Transaction>()
                .ConstructUsing(src => new Transaction(src.Amount, src.FromAccountId, src.ToAccountId, src.Date))
                .ForMember(t => t.Id, opt => opt.MapFrom(src => src.Id));
        }
        
    }
}
