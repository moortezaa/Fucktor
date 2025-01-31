using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

namespace OnlinePayment
{
    public static class OnlinePaymentServiceProvider
    {
        public static void AddOnlinePayment(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<PaymentDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<OnlinePayment>();
        }
        public static void UseOnlinePayment(this WebApplication app)
        {
            using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var serviceProvider = serviceScope.ServiceProvider;
                var context = serviceProvider.GetService<PaymentDbContext>();
                if (!app.Environment.IsDevelopment())
                {
                    context.Database.Migrate();
                }
            }
        }

    }
}
