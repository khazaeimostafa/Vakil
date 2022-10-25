using Met.DTOs;
using Met.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters
{
    public class LoginValidationAttribute : IAsyncActionFilter
    {
        private readonly UserManager<AppUser> userManager;

        public LoginValidationAttribute(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }


        public async void OnActionExecuting(ActionExecutingContext context)
        {

 

            //var action = context.RouteData.Values["action"];


            //string? param = (string)context.ActionArguments["EmailAddress"];

            //if (param == null)
            //{
            //    context.Result = new OkObjectResult(param);
            //}
            //else
            //{
            //    context.Result = new OkObjectResult("fffffff");
            //}

        }


        public async void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            if (!context.ModelState.IsValid)
            {
                context.Result = new UnprocessableEntityObjectResult(context.ModelState);
            }



            #region MyRegion

            //Dictionary<string, object?> arg = new Dictionary<string, object?>();
            //foreach (KeyValuePair<string, object?> item in context.ActionArguments)
            //{
            //    arg.Add(item.Key, item.Value);
            //} 

            //AppUser user = await userManager.FindByEmailAsync((string)arg["EmailAddress"].ToString());


            #endregion


            LoginVM mail = (LoginVM)context.ActionArguments["model"];


            if (mail == null)
            {
                context.Result = new BadRequestObjectResult(mail.EmailAddress);
                return;
            }

            AppUser user = await userManager.FindByEmailAsync(mail.EmailAddress);
            
            if (user == null)
            {
                context.Result = new BadRequestObjectResult("User Not Exist");
                return;
            }


            if (user != null && await userManager.IsLockedOutAsync(user))
            {
                context.Result = new BadRequestObjectResult("Your Account is Locked");
                return;
            }

            if (user != null && await userManager.IsEmailConfirmedAsync(user))
            {
                context.Result = new BadRequestObjectResult("You Should Confirm Your Account");
                return;
            }

            else
            {
                context.HttpContext.Items.Add("userExist", user);
                await next();
            }

        }
    }
}
