using Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data
{
    public class GenerateUser
    {
        private readonly UserManager<IdentityUser> _userManager;
        public GenerateUser(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task CreateUser()
        {
            var user = new IdentityUser
            {
                UserName = "admin",
                Email = "admin@email"
            };
            await _userManager.CreateAsync(user, "123");
            await _userManager.AddToRoleAsync(user, ApplicationRoles.Admin);
        }
    }
}
