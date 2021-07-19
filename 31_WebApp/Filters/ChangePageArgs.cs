using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApp.Filters
{
    public class ChangePageArgs : Attribute, IPageFilter
    {
        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            // do nothing
        }

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            if (context.HandlerArguments.ContainsKey("message1"))
            {
                context.HandlerArguments["message1"] = "Changed message1 in ChangePageArgs.OnPageHandlerExecuting() method!!";
            }
        }

        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
            // do nothing
        }
    }
}
