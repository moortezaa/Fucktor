using Core;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    internal class AppRoleRepository : GeneralRepository<SqlDbContext, AppRole, Guid>, IAppRoleRepository
    {
        public AppRoleRepository(SqlDbContext sqlDbContext) : base(sqlDbContext)
        {
        }

        public void AddRolePermission(RolePermission rolePermission)
        {
            _context.RolePermissions.Add(rolePermission);
        }

        public async Task<AppRole?> GetRoleByNameAsync(string roleName)
        {
            return await _context.AspNetRoles.SingleOrDefaultAsync(r => r.Name == roleName);
        }

        public Task<AppRole?> GetRoleByNameIncludePermissionsAsync(string roleName)
        {
            return _context.AspNetRoles
                .Include(r=>r.Permissions)
                .ThenInclude(p=>p.Permission)
                .SingleOrDefaultAsync(r => r.Name == roleName);
        }

        public async Task<List<AppUser>> GetUsersInRole(Guid id)
        {
            var query = from ur in _context.AspNetUserRoles
                        join u in _context.AspNetUsers on ur.UserId equals u.Id
                        where ur.RoleId == id
                        select u;
            return await query.ToListAsync();
        }

        public async Task<bool> IsAnyUserInRole(Guid id)
        {
            return await _context.AspNetUserRoles
                .Where(ur => ur.RoleId == id)
                .AnyAsync();
        }

        public void RemoveRolePermission(RolePermission rolePermission)
        {
            _context.RolePermissions.Remove(rolePermission);
        }
    }
}