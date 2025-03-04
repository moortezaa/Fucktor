using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core
{
    public class AppUser : IdentityUser<Guid>
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? NationalCode { get; set; }
        public string? DisplayName { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }

        public string? CompanyName { get; set; }
        public string? EconomicCode { get; set; }
        public string? RegistrationNumber { get; set; }

        public Gender Gender { get; set; } = Gender.NotSet;
        public LegalPersonType LegalPersonType { get; set; } = LegalPersonType.NaturalPerson;


        public List<GatewayAccount> GatewayAccounts { get; set; } = new List<GatewayAccount>();

        [NotMapped]
        public List<string> RoleNames { get; set; } = new List<string>();
    }
}