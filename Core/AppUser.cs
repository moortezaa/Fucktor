using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core
{
    public class AppUser : IdentityUser<Guid>
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? NationalCode { get; set; }

        public Gender Gender { get; set; } = Gender.NotSet;

        public string? DisplayName { get; set; }

        public List<GatewayAccount> GatewayAccounts { get; set; } = new List<GatewayAccount>();

        [NotMapped]
        public List<string> RoleNames { get; set; } = new List<string>();
    }
}