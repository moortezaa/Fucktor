using Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class SqlDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetPrecision(28);
                property.SetScale(0);
            }
        }

        public DbSet<AppRole> AspNetRoles { set; get; }
        public DbSet<IdentityUserRole<Guid>> AspNetUserRoles { set; get; }
        public DbSet<AppUser> AspNetUsers { set; get; }
        public DbSet<Permission> Permissions { set; get; }
        public DbSet<RolePermission> RolePermissions { set; get; }
        public DbSet<GatewayAccount> GatewayAccounts { get; set; }
    }
}