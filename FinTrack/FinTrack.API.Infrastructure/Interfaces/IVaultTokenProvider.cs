
using FinTrack.API.Infrastructure.Identity.DTO;
using Microsoft.Extensions.Options;

namespace FinTrack.API.Infrastructure.Interfaces
{
    public interface IVaultTokenProvider
    {
        Task<string?> GetVaultToken();
    }
}
