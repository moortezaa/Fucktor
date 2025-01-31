using System.ComponentModel.DataAnnotations;

namespace Core
{
    public class Permission
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Group { get; set; }
        public List<RolePermission> Roles { get; set; } = new List<RolePermission>();
    }
}