using Core;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    internal class AppUserRepository : GeneralRepository<SqlDbContext, AppUser, Guid>, IAppUserRepository
    {
        public IQueryable<AppUser> AppUserQuery { get; }
        public AppUserRepository(SqlDbContext sqlDbContext) : base(sqlDbContext)
        {
            AppUserQuery = sqlDbContext.AspNetUsers;
        }

        public async Task<AppUser?> GetByUserNameAsync(string userName)
        {
            return await _context.AspNetUsers.Where(x => x.UserName == userName).SingleOrDefaultAsync();
        }

        public Task<List<AppRole>> GetUserRoles(Guid id)
        {
            var query = from ur in _context.AspNetUserRoles
                        join r in _context.AspNetRoles on ur.RoleId equals r.Id
                        where ur.UserId == id
                        select r;
            return query.ToListAsync();
        }

        public Task<List<Permission>> GetUserPermissions(Guid id)
        {
            var query = (from p in _context.Permissions
                         join rp in _context.RolePermissions on p.Id equals rp.PermissionId
                         join ur in _context.AspNetUserRoles on rp.RoleId equals ur.RoleId
                         where ur.UserId == id
                         select p).Distinct();
            return query.ToListAsync();
        }

        public Task<AppUser?> GetUserByPhoneNumberAndNationalCode(string doctorPhone, string doctorNationalCode)
        {
            return _context.AspNetUsers.Where(u => u.PhoneNumber == doctorPhone && u.NationalCode == doctorNationalCode).FirstOrDefaultAsync();
        }
    }
}