using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure.Services;
using Met.DTOs;
using Met.Data;
using Met.Entities;
using Met.MetExtensions;
using Met.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    [Route("[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly ILogger<ProfileController> _logger;

        private readonly UserManager<AppUser> userManager;

        private readonly AppIdentityDbContext context;

        private readonly IdentityEmailService EmailService;

        private readonly TokenUrlEncoderService TokenUrlEncoder;

        private readonly SignInManager<AppUser> SignInManager;

        private readonly IConfiguration configuration;

        private readonly TokenValidationParameters tokenValidationParameters;

        public ProfileController(
            UserManager<AppUser> userManager,
            ILogger<ProfileController> logger,
            AppIdentityDbContext context,
            IdentityEmailService emailService,
            TokenUrlEncoderService encoder,
            SignInManager<AppUser> signinManager,
            IConfiguration configuration,
            TokenValidationParameters tokenValidationParameters
        )
        {
            _logger = logger;
            this.userManager = userManager;
            this.context = context;
            this.EmailService = emailService;
            this.TokenUrlEncoder = encoder;
            this.SignInManager = signinManager;
            this.configuration = configuration;
            this.tokenValidationParameters = tokenValidationParameters;
        }

        [HttpGet]
        [Route("User-Claims")]
        public async Task<ActionResult<IList<Claim>>>
        GetUserClaims(string model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            AppUser appUser = await userManager.FindByEmailAsync(model);
            if (appUser == null)
            {
                return BadRequest("User Not Exist");
            }

            IList<Claim> Claims = await userManager.GetClaimsAsync(appUser);

            return Ok(Claims);
        }

        [HttpGet]
        [Route("Add-Claims")]
        public async Task<ActionResult<IList<Claim>>>
        ClaimAdd(ClaimAddDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            AppUser appUser = await userManager.FindByEmailAsync(model.Email);
            if (appUser == null)
            {
                return BadRequest("User Not Exist");
            }

            IdentityResult result =
                await userManager
                    .AddClaimAsync(appUser,
                    new Claim(model.ClaimType, model.ClaimValue));
            if (!result.Succeeded)
            {
                return BadRequest("Error in Adding New Claim");
            }
            return Ok(await userManager.GetClaimsAsync(appUser));
        }

        [HttpGet]
        [Route("Cet-Current-User")]
        public async Task<ActionResult<UserAuthBase>> GetCurrenUser()
        {
            var user =
                await userManager.FindUserByEMailExtension(HttpContext.User);

            if (user == null)
            {
                return BadRequest("User Not Exist");
            }
            UserAuthBase userAuthBase =
                new UserAuthBase {
                    UserId = user.Id,
                    Claims = (await userManager.GetClaimsAsync(user)).ToList(),
                    IsAuthenticated = true,
                    UserName = user.UserName,
                    appUserAuth =
                        await user
                            .GenerateJWTTokenAsync(userManager,
                            context,
                            configuration,
                            null)
                };

            return Ok(userAuthBase);
        }

        [HttpPost]
        [Route("Delete-Account")]
        public async Task<ActionResult> OnPostDeleteAccountAsync(string model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userManager.FindByIdAsync(model);
            if (user == null)
            {
                return BadRequest("This Account Not Exist");
            }

            IList<Claim> claims = await userManager.GetClaimsAsync(user);

            await userManager.RemoveClaimsAsync(user, claims);

            List<FreshToken> Tokens =
                context.RefreshTokens.Where(x => x.UserId == model).ToList();
            context.RefreshTokens.RemoveRange (Tokens);

            IdentityResult result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest("Deleting Account Failed");
            }
            await context.SaveChangesAsync();
            return Ok($"User With {  user.Id.ToString()} Id Removed Successfully");
        }

        // [Authorize(Policy = "Admin")]
        // [Authorize(Policy = "EditAdmin")]
        [HttpGet]
        [Route("Delete-Self-Account")]
        public async Task<IActionResult> OnPostSelfDeleteAccountAsync()
        {
            var user =
                await userManager.FindUserByEMailExtension(HttpContext.User);

            if (user == null)
            {
                return BadRequest("This Account Not Exist");
            }

            IList<Claim> claims = await userManager.GetClaimsAsync(user);

            await userManager.RemoveClaimsAsync(user, claims);

            List<FreshToken> Tokens =
                context.RefreshTokens.Where(x => x.UserId == user.Id).ToList();
            context.RefreshTokens.RemoveRange (Tokens);

            IdentityResult result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest("Deleting Account Failed");
            }
            await context.SaveChangesAsync();
            return Ok($"User With {  user.Id.ToString()} Id Removed Successfully");
        }
    }
}
