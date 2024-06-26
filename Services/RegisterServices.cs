using Services.Interfaces;
using Services.Implement;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Core.Settings;

namespace Services
{
    public static class RegisterServices
    {
        public static void RegisterAppServices(this IServiceCollection services)
        {
            
            services.AddScoped<IInventoryService, InventoryService>();

            services.AddScoped<IShopService, ShopService>();

            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<IOrderService, OrderService>();
        }
    }
}
