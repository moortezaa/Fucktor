using Core;

namespace Repository
{
    public interface IAppRoleRepository : IGeneralRepository<AppRole, Guid>
    {
        void AddRolePermission(RolePermission rolePermission);
        Task<AppRole?> GetRoleByNameAsync(string roleName);
        Task<AppRole?> GetRoleByNameIncludePermissionsAsync(string roleName);
        Task<List<AppUser>> GetUsersInRole(Guid id);
        Task<bool> IsAnyUserInRole(Guid id);
        void RemoveRolePermission(RolePermission rolePermission);
    }
}
