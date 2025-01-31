using System.ComponentModel.DataAnnotations;

namespace Core
{
    public class RolePermission
    {
        public Guid Id { get; set; }

        [Required]
        public Guid RoleId { get; set; } = Guid.Empty;
        public AppRole? Role { get; set; }

        [Required]
        public Guid PermissionId { get; set; } = default;
        public Permission? Permission { get; set; }
    }
}