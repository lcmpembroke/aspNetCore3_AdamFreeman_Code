using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace WebApp.Filters
{

    public class SimpleCacheAttribute : Attribute, IAsyncResourceFilter
    {
        private Dictionary<PathString, IActionResult> CachedResponses = new Dictionary<PathString, IActionResult>();

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            PathString path = context.HttpContext.Request.Path;

            if (CachedResponses.ContainsKey(path))
            {
                context.Result = CachedResponses[path];
                CachedResponses.Remove(path);
            }
            else
            {
                ResourceExecutedContext executedContext = await next();
                CachedResponses.Add(context.HttpContext.Request.Path, executedContext.Result);
            }
        }
    }

    // BELOW WAS SYNCHRONOUS
    //public class SimpleCacheAttribute :Attribute, IResourceFilter
    //{
    //    private Dictionary<PathString, IActionResult> CachedResponses = new Dictionary<PathString, IActionResult>();

    //    public void OnResourceExecuting(ResourceExecutingContext context)
    //    {
    //        PathString path = context.HttpContext.Request.Path;

    //        if (CachedResponses.ContainsKey(path))
    //        {
    //            context.Result = CachedResponses[path];
    //            CachedResponses.Remove(path);
    //        }
    //    }

    //    public void OnResourceExecuted(ResourceExecutedContext context)
    //    {
    //        CachedResponses.Add(context.HttpContext.Request.Path, context.Result);
    //    }
    //}
}
