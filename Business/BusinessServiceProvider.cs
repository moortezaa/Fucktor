using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Business.Attributes;
using Repository;
using Core;
using Data;
using OnlinePayment;

namespace Business
{
    public static class BusinessServiceProvider
    {
        public static void AddBusiness(this IServiceCollection services, string connectionString)
        {
            services.AddRepository(connectionString);
            services.AddOnlinePayment(connectionString);
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.RequireUniqueEmail = false;

                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;

            })
                .AddEntityFrameworkStores<SqlDbContext>()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<AppIdentityErrorDescriber>();
            services.AddScoped<PermissionManager>();
            services.AddScoped<AppRoleManager>();
            services.AddScoped<AppUserManager>();
            services.AddScoped<GatewayAccountManager>();
            services.AddScoped<InvoiceManager>();
            services.AddScoped<ItemManager>();

            //services.AddAuthentication()
            //    .AddIdentityCookies();

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(1);

                options.LoginPath = "/User/SignIn";
                options.AccessDeniedPath = "/User/AccessDenied";
                options.SlidingExpiration = true;
            });
        }

        public static void UseBusiness(this WebApplication app)
        {
            app.UseAuthentication();

            var permissionSeedModels = new List<PermissionSeedModel>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    //foreach method in the type if it has the PermissionAttribute check if the type is a controller
                    foreach (var method in type.GetMethods())
                    {
                        var permissionAttribute = method.GetCustomAttribute<PermissionAttribute>();
                        if (permissionAttribute != null)
                        {
                            if (!type.IsSubclassOf(typeof(ControllerBase)))
                            {
                                throw new ApplicationException($"PermissionAttribute is applied to a method in type \"{type}\" which is not a Controller.");
                            }
                            permissionSeedModels.Add(new PermissionSeedModel(permissionAttribute.Permission, type.FullName, permissionAttribute.IsAdmin));
                        }
                    }
                }
            }

            using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var serviceProvider = serviceScope.ServiceProvider;
                var context = serviceProvider.GetService<SqlDbContext>();
                if (!app.Environment.IsDevelopment())
                {
                    context.Database.Migrate();
                }
                var permissionManager = serviceProvider.GetService<PermissionManager>()!;
                var appUserManager = serviceProvider.GetService<AppUserManager>()!;
                var appRoleManager = serviceProvider.GetService<AppRoleManager>()!;

                // Check if the permission exists, then add it if it doesn't
                foreach (var model in permissionSeedModels)
                {
                    var result = permissionManager.AddPermission(model.Title, model.Group).Result; //this method does not add the permission if it already exists
                    if (!result.Succeeded)
                    {
                        if (result.Errors.Any(e => e == "Permission already exists"))
                        {
                            continue;
                        }
                        throw new ApplicationException($"Error adding permission: {string.Join("|", result.Errors)}");
                    }
                }

                // Check if the admin user exists, then add them if they don't
                if (appUserManager.GetUserByUserName("admin").Result == null)
                {
                    appUserManager.AddUser(new AppUser()
                    {
                        UserName = "admin",
                        Name = "Morteza",
                        LastName = "Amini"
                    }, "M.amini@1378").Wait();
                }
                var admin = appUserManager.GetUserByUserName("admin").Result!;
                //reset admin password
                appUserManager.ChangePassword(admin.Id, "M.amini@1378").GetAwaiter().GetResult();

                // Check if the Admin role exists, then add it if it doesn't
                var adminRole = appRoleManager.GetRoleByName("Admin").Result;
                var rolePermissions = new List<Permission>();
                if (adminRole == null)
                {
                    appRoleManager.AddRole("Admin").Wait();
                }
                else
                {
                    rolePermissions = appRoleManager.GetPermissionsForRole(adminRole.Name!).Result;
                }

                // Check if the admin user exists in the role, then add them if they don't
                if (!appUserManager.GetUserRoles(admin.Id).Result.Any(r => r.Name == "Admin"))
                {
                    appUserManager.AddUserToRole(admin.Id, "Admin").Wait();
                }

                foreach (var model in permissionSeedModels.Where(p => p.IsAdmin))
                {
                    if (rolePermissions.Any(p => p.Title == model.Title))
                    {
                        continue;
                    }
                    appRoleManager.AddPermissionToRole("Admin", model.Title).Wait();
                }

                // Check if the Seller role exists, then add it if it doesn't
                var sellerRoleName = "Seller";
                var sellerRole = appRoleManager.GetRoleByName(sellerRoleName).Result;
                rolePermissions = [];
                if (sellerRole == null)
                {
                    appRoleManager.AddRole(sellerRoleName).Wait();
                }
                else
                {
                    rolePermissions = appRoleManager.GetPermissionsForRole(sellerRole.Name!).Result;
                }

                // Add Seller Permissions
                var sellerPermissions = new string[]
                {
                    "ViewHome",
                    "UseDashboard",
                    "GetInvoiceTable",
                    "ViewInvoices",
                    "ViewInvoiceDetails",
                    "EditInvoice",
                    "EditUser",
                    "ViewUserDetail",
                    "VerifyPhoneNumber",
                    "ChangePassword",
                    "EnableTwoFactor",
                    "EditInvoiceItem",
                };
                foreach (var permission in sellerPermissions)
                {
                    if (rolePermissions.Any(p => p.Title == permission))
                    {
                        continue;
                    }
                    appRoleManager.AddPermissionToRole(sellerRoleName, permission).Wait();
                }
                
                // Check if the Seller role exists, then add it if it doesn't
                var buyerRoleName = "Buyer";
                var buyerRole = appRoleManager.GetRoleByName(buyerRoleName).Result;
                rolePermissions = [];
                if (buyerRole == null)
                {
                    appRoleManager.AddRole(buyerRoleName).Wait();
                }
                else
                {
                    rolePermissions = appRoleManager.GetPermissionsForRole(buyerRole.Name!).Result;
                }

                // Add Seller Permissions
                var buyerPermissions = new string[]
                {
                    "ViewHome",
                    "UseDashboard",
                    "GetInvoiceTable",
                    "EditUser",
                    "ViewUserDetail",
                    "VerifyPhoneNumber",
                    "ChangePassword",
                    "EnableTwoFactor",
                };
                foreach (var permission in buyerPermissions)
                {
                    if (rolePermissions.Any(p => p.Title == permission))
                    {
                        continue;
                    }
                    appRoleManager.AddPermissionToRole(buyerRoleName, permission).Wait();
                }

                app.UseOnlinePayment();
            }
        }
    }
}
