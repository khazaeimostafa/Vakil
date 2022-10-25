using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Core.ServicesInterfaces;
using Infrastructure.Services;
using Met.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;

namespace Met.Services
{
    public class IdentityEmailService
    {
        public IEmailSender EmailSender { get; set; }

        public UserManager<AppUser> UserManager { get; set; }

        public IHttpContextAccessor ContextAccessor { get; set; }

        public LinkGenerator LinkGenerator { get; set; }

        public TokenUrlEncoderService TokenEncoder { get; set; }

        public IdentityEmailService(
            IEmailSender sender,
            UserManager<AppUser> userMgr,
            IHttpContextAccessor contextAccessor,
            LinkGenerator generator,
            TokenUrlEncoderService encoder
        )
        {
            EmailSender = sender;
            UserManager = userMgr;
            ContextAccessor = contextAccessor;
            LinkGenerator = generator;
            TokenEncoder = encoder;
        }

        public async Task<string>
        SendAccountConfirmEmail(
            AppUser user,
            string ConfirmAction,
            string ConfirmController
        )
        {
            string token =
                await UserManager.GenerateEmailConfirmationTokenAsync(user);

            string url =
                GetUrl(user.Email, token, ConfirmAction, ConfirmController);

            await EmailSender
                .SendEmailAsync(user.Email,
                "Complete Your Account Setup",
                $"Please set up your account by <a href={url}>clicking here</a>.");

            return url;
        }

        private string
        GetUrl(
            string emailAddress,
            string token,
            string action,
            string controller
        )
        {
            string safeToken = TokenEncoder.EncodeToken(token);

            return LinkGenerator
                .GetUriByAction(ContextAccessor.HttpContext,
                action,
                controller,
                new { email = emailAddress, token = safeToken });
        }
    }
}
