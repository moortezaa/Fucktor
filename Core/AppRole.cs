using Microsoft.AspNetCore.Identity;

namespace Core
{
    public class AppRole:IdentityRole<Guid>
    {
        public List<RolePermission> Permissions { get; set; } = new List<RolePermission>();
    }
}
