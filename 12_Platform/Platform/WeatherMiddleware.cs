using Microsoft.AspNetCore.Http;
using Platform.Services;
using System.Threading.Tasks;

namespace Platform
{
    public class WeatherMiddleware
    {
        private RequestDelegate next;
        private IResponseFormatter formatter;

        public WeatherMiddleware(RequestDelegate nextDelegate, IResponseFormatter responseFormatter)
        {
            next = nextDelegate;
            formatter = responseFormatter;

        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == "/middleware/class")
            {
                //await context.Response.WriteAsync("WeatherMiddleware middleware class; it's raining in London");
                await formatter.Format(context, "WeatherMiddleware middleware class USING formatter SERVICE; it's raining in London");
            }
            else
            {
                await next(context);
            }
        }
    }
}
