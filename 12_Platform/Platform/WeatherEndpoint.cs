using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Platform.Services;
using System;
using System.Threading.Tasks;

namespace Platform
{
    public class WeatherEndpoint
    {
        // Commented out to move the dependency on IResponseFormatter FROM the constructor TO the EndPoint method  - so a new service object will be received for every request
        //private IResponseFormatter formatter;

        //public WeatherEndpoint(IResponseFormatter responseFormatter)
        //{
        //    formatter = responseFormatter;
        //}

        public async Task Endpoint(HttpContext httpContext, IResponseFormatter formatter)
        {
            Console.WriteLine(formatter.GetType().CustomAttributes.ToString());
            await formatter.Format(httpContext, "WeatherEndpoint class no longer static. \nDependency on IResponseFormatter service through WeatherEndpoint.Endpoint() method \n(not via constructor injection): it's cloudy in Milan");
        }
    }
}
