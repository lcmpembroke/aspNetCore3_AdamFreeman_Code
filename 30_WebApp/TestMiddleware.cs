using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp
{
    public class TestMiddleware
    {
        private RequestDelegate next;

        public TestMiddleware(RequestDelegate nextDelegate)
        {
            next = nextDelegate;
        }

        public async Task Invoke(HttpContext httpContext, DataContext dataContext)
        {
            if (httpContext.Request.Path == "/test")
            {
                await httpContext.Response.WriteAsync($"There are {dataContext.Products.Count()} Products \n");
                await httpContext.Response.WriteAsync($"There are {dataContext.Categories.Count()} Categories\n");
                await httpContext.Response.WriteAsync($"There are {dataContext.Suppliers.Count()} Suppliers\n");
            }
            else
            {
                await next(httpContext);
            }
        }
    }
}
