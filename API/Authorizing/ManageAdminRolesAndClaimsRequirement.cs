using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Authorizing
{
    public class
    ManageAdminRolesAndClaimsRequirement : IAuthorizationRequirement
    { }

    public class
    canEditOnlyOtherAdminRolesAndClaimsHandler
    : AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {
        protected override Task
        HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ManageAdminRolesAndClaimsRequirement requirement
        )
        {
            AuthorizationFilterContext? authFilterContext =
                context.Resource as AuthorizationFilterContext;

            if (authFilterContext == null) return Task.CompletedTask;

            string? loggedInAdminId =
                context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            string adminBeingEdited =
                authFilterContext.HttpContext.Request.Query["Id"];

            if (
                context.User.IsInRole("Admin") &&
                context
                    .User
                    .HasClaim(c => c.Type == "EditRole" && c.Value == "true") &&
                (adminBeingEdited.ToLower() != loggedInAdminId.ToLower())
            )
            {

                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
