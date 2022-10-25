using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helper;
using Microsoft.AspNetCore.Identity;

namespace API.Seed
{
 public static class AppInitializer
    {

        public static async Task SeedRolesToDb(this IApplicationBuilder webApplication)
        {
            using (var ScopeService = webApplication.ApplicationServices.CreateScope())
            {
                var roleManager = ScopeService.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                if (!await roleManager.RoleExistsAsync(UserRoles.Manager))
                {
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Manager));
                }

                if (!await roleManager.RoleExistsAsync(UserRoles.Student))
                {
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Student));
                }

            }
        }
    }
}
