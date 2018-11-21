using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RC.API.Filter
{
    public class AuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                if (!context.Filters.Any(row => row is AllowAnonymousFilter) &&
                    !(context.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo.GetCustomAttributes(true)
                        .Any(row => row is AllowAnonymousAttribute) == true)
                {
                    if(!context.HttpContext.User.Identity.IsAuthenticated){
                        context.Result = new UnauthorizedResult();
                    }
                }
            }
            catch (Exception) 
            {
                context.Result = new BadRequestResult();
            }

            base.OnActionExecuting(context);
        }
    }
}
