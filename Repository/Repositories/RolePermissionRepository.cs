using Core;
using Data;

namespace Repository
{
    internal class RolePermissionRepository : GeneralRepository<SqlDbContext, RolePermission, Guid>, IRolePermissionRepository
    {
        public RolePermissionRepository(SqlDbContext sqlDbContext) : base(sqlDbContext)
        {
        }
    }
}