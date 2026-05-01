namespace FinTrack.API.Infrastructure.Identity.DTO
{
    public class VaultOptions
    {
        public string VaultAddress {get;set;} = null!;
        public string RoleID {get;set;} = null!;
        public string SecretID {get;set;} = null!;
        public string KeyName {get;set; } = null!;

        
    }

}