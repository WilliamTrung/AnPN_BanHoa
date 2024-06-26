using Core.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    public static class RoleInitializer
    {
        private static readonly List<IdentityRole> Roles = new List<IdentityRole>()
        {
            new IdentityRole { Name = ApplicationRoles.Admin, NormalizedName = ApplicationRoles.Admin.ToUpper(), ConcurrencyStamp = Guid.NewGuid().ToString() },
            new IdentityRole { Name = ApplicationRoles.Member, NormalizedName = ApplicationRoles.Member.ToUpper(), ConcurrencyStamp = Guid.NewGuid().ToString()}
        };
        public static async Task SeedData(this IServiceProvider serviceProvider)
        {
            using (var dbContext = serviceProvider.GetRequiredService<StoreDbContext>())
            {
                dbContext.Database.EnsureCreated();
            }
            using (var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>())
            {
                dbContext.Database.EnsureCreated();
                AddRoles(dbContext);
                using (var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>())
                {
                    await AddUsersAsync(userManager);
                }
            }
        }
        private static void AddRoles(ApplicationDbContext dbContext)
        {
            if (dbContext.Roles.Any()) return;

            foreach (var role in Roles)
            {
                dbContext.Roles.Add(role);
                dbContext.SaveChanges();
            }
        }
        private static async Task AddUsersAsync(UserManager<ApplicationUser> userManager)
        {
            var user = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@email",
                Address = "address",
                Address2 = "address2",
                City="hcm",
                FirstName="admin",
                LastName="admin",
                State = "admin",
                Zip = "60000",
            };
            var result = await userManager.CreateAsync(user, "123");
            await userManager.AddToRoleAsync(user, ApplicationRoles.Admin);
        }
    }
}
