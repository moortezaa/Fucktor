using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class GatewayAccount
    {
        public Guid Id { get; set; }
        public GatewayType Type { get; set; }
        public string? Name { get; set; }
        public string? MerchantId { get; set; }
        public string? AuthorizationToken { get; set; }
        public string? Password { get; set; }

        public Guid UserId { get; set; }
        public AppUser? User { get; set; }
    }
}
