using Core.Data;
using Core.Models;
using Core.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core
{
    public static class RegisterCore
    {
        public static void RegisterAppCore(this IServiceCollection services,in IConfiguration configuration)
        {
            var mailsettings = new MailSetting();
            services.Configure<MailSetting>(configuration.GetSection("MailSettings"));
            string storeConnection = configuration.GetConnectionString("storedb");
            string userConnection = configuration.GetConnectionString("userdb");

            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(userConnection));

            services.AddDbContext<StoreDbContext>(options =>
                options.UseSqlServer(storeConnection));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 1;
            })
                 .AddEntityFrameworkStores<ApplicationDbContext>()
                 .AddDefaultTokenProviders();

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("AdminOnly", policy => policy.RequireRole(ApplicationRoles.Admin));
            //});
        }
    }
}
