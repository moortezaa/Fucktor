using Core;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    internal class PermissionRepository : GeneralRepository<SqlDbContext, Permission, Guid>, IPermissionRepository
    {
        public PermissionRepository(SqlDbContext sqlDbContext) : base(sqlDbContext)
        {
        }

        public void AddRange(List<Permission> permissions)
        {
            _context.Permissions.AddRange(permissions);
        }

        public async Task<Permission?> GetPermissionByTitle(string permissionName)
        {
            return await _context.Permissions.SingleOrDefaultAsync(p => p.Title == permissionName);
        }
    }
}