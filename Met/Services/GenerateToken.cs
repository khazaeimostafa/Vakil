using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Met.DTOs;
using Met.Data;
using Met.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Met.Services
{
    public static class GenerateToken
    {
        public static async Task<AuthResultVM>
        GenerateJWTTokenAsync(
            this AppUser user,
            UserManager<AppUser> userManager,
            AppIdentityDbContext context,
            IConfiguration configuration,
            FreshToken refToken
        )
        {
            var authClaims =
                new List<Claim> {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,
                        Guid.NewGuid().ToString())
                };

            IList<Claim> userClaims = await userManager.GetClaimsAsync(user);

            foreach (Claim item in userClaims)
            {
                authClaims.Add(item);
            }

            IList<string> userRoles = await userManager.GetRolesAsync(user);

            foreach (string item in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, item));
            }

            SymmetricSecurityKey authSigningKey =
                new SymmetricSecurityKey(Encoding
                        .ASCII
                        .GetBytes(configuration["JWT:secret"]));

            JwtSecurityToken token =
                new JwtSecurityToken(issuer: configuration["JWT:Issuer"],
                    audience: configuration["JWT:Audience"],
                    claims: authClaims,
                    expires: DateTime.Now.AddDays(7),
                    signingCredentials: new SigningCredentials(authSigningKey,
                        SecurityAlgorithms.HmacSha512Signature));

            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            if (refToken != null)
            {
                AuthResultVM RefreshTokenResponse =
                    new AuthResultVM()
                    {
                        Token = jwtToken,
                        ExpiresAt = token.ValidTo,
                        RefreshToken = refToken.Token
                    };

                return RefreshTokenResponse;
            }

            FreshToken refreshToken =
                new FreshToken()
                {
                    JwtId = token.Id,
                    IsRevoked = false,
                    UserId = user.Id,
                    DateExpire = DateTime.UtcNow.AddMonths(6),
                    DateAdded = DateTime.UtcNow,
                    Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
                };

            await context.RefreshTokens.AddAsync(refreshToken);

            await context.SaveChangesAsync();

            AuthResultVM response =
                new AuthResultVM()
                {
                    Token = jwtToken,
                    RefreshToken = refreshToken.Token,
                    ExpiresAt = token.ValidTo
                };

            return response;
        }
    }
}
