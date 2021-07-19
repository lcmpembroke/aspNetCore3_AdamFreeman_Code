using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApp.Filters
{
    // p792
    public class ChangeArgAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments.ContainsKey("message1"))
            {
                context.ActionArguments["message1"] = "New message";
            }
            await next();
        }
    }

    // page 791
    //public class ChangeArgAttribute : Attribute, IAsyncActionFilter
    //{
    //    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    //    {
    //        if (context.ActionArguments.ContainsKey("message1"))
    //        {
    //            context.ActionArguments["message1"] = "New message";
    //        }
    //        await next();
    //    }
    //}
}
