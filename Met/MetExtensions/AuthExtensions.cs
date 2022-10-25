using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Met.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Met.MetExtensions
{
    public static class AuthExtensions
    {
        public static async Task<AppUser>
        FindUserByEMailExtension(
           this UserManager<AppUser> userManager, ClaimsPrincipal user
        )
        {
            var email =
                user?
                    .Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.Email)?
                    .Value;

            return await userManager
                .Users
                .SingleOrDefaultAsync(x => x.Email == email);
        }

         public static async Task<bool> CheckEmailExistsAsync(this UserManager<AppUser> userManager, string EmailAddress)
        {

            return await userManager.FindByEmailAsync(EmailAddress) != null ? true : false;
        }
        
    }
}
