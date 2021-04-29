using Microsoft.AspNetCore.Http;
using Platform.Services;
using System.Threading.Tasks;

namespace Platform
{
    public class WeatherMiddleware
    {
        private RequestDelegate next;
        //private IResponseFormatter formatter;

        public WeatherMiddleware(RequestDelegate nextDelegate)
        {
            next = nextDelegate;
            //formatter = responseFormatter;

        }

        // ASP.NET Core resolves dependencies declared by Invoke() method every time a request in processed
        // this ensures a new transient service object is created
        public async Task Invoke(HttpContext context, IResponseFormatter formatter)
        {
            if (context.Request.Path == "/middleware/class")
            {
                //await context.Response.WriteAsync("WeatherMiddleware middleware class; it's raining in London");
                await formatter.Format(context, "WeatherMiddleware middleware class USING formatter SERVICE through injected in WeatherMiddleware.Invoke() method; it's raining in London");
            }
            else
            {
                await next(context);
            }
        }
    }
}
