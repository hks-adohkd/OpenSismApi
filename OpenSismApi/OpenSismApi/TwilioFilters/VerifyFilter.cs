using DBContext.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace OpenSismApi.TwilioFilters
{
    public class VerifyFilter : IAsyncResourceFilter
    {

        private readonly UserManager<ApplicationUser> _manager;

        public VerifyFilter(UserManager<ApplicationUser> manager)
        {
            _manager = manager;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                throw new Exception("Authentication required before verification");
            }

            var user = await _manager.GetUserAsync(context.HttpContext.User);
            if (!user.PhoneNumberConfirmed)
            {
                context.Result = new RedirectResult("/Identity/Account/Verify");
            }
            else
            {
                await next();
            }
        }
    }
}
