using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Met.DTOs;
using Met.Data;
using Met.Entities;
using Met.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Met.MetExtensions
{
    public static class RefreshTokenExtension
    {
        public static async Task<AuthResultVM>
        VerifyAndGenerateTokenAsync(
            this UserManager<AppUser> userManager,
            TokenRequestVM model,
            AppIdentityDbContext context,
            TokenValidationParameters tokenValidationParameters,
            IConfiguration configuration
        )
        {
            JwtSecurityTokenHandler jwtTokenHandler =
                new JwtSecurityTokenHandler();

            FreshToken storeToken =
                await context
                    .RefreshTokens
                    .FirstOrDefaultAsync(x => x.Token == model.RefreshToken);

            AppUser user = await userManager.FindByIdAsync(storeToken.UserId);

            try
            {
                var tokenCheckResult =
                    jwtTokenHandler
                        .ValidateToken(model.Token,
                        tokenValidationParameters,
                        out var validateToken);

                return await user
                    .GenerateJWTTokenAsync(userManager,
                    context,
                    configuration,
                    storeToken);
            }
            catch (SecurityTokenExpiredException)
            {
                if (storeToken.DateExpire > DateTime.UtcNow)
                {
                    return await user
                        .GenerateJWTTokenAsync(userManager,
                        context,
                        configuration,
                        storeToken);
                }
                else
                {
                    return await user
                        .GenerateJWTTokenAsync(userManager,
                        context,
                        configuration,
                        null);
                }
            }
        }
    }
}
