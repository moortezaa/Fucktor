using Data;
using Microsoft.Extensions.DependencyInjection;

namespace Repository
{
    public static class RepositoryServiceProvider
    {
        public static void AddRepository(this IServiceCollection services, string connectionString)
        {
            services.AddSQLDatabase(connectionString);
            services.AddScoped<IAppRoleRepository, AppRoleRepository>();
            services.AddScoped<IAppUserRepository, AppUserRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<IGatewayAccountRepository, GatewayAccountRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IInvoiceItemRepository, InvoiceItemRepository>();
        }
    }
}
