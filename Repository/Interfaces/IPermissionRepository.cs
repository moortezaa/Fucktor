using Core;

namespace Repository
{
    public interface IPermissionRepository : IGeneralRepository<Permission, Guid>
    {
        void AddRange(List<Permission> permissions);
        Task<Permission?> GetPermissionByTitle(string permissionName);
    }
}
