using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Data
{
    public static class DatabaseServiceProvider
    {
        public static void AddSQLDatabase(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<SqlDbContext>(options =>
                options.UseSqlServer(connectionString));
        }
    }
}
