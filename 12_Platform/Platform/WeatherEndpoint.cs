using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Platform.Services;
using System.Threading.Tasks;

namespace Platform
{
    public class WeatherEndpoint
    {
        // Commented out to move the dependency on IResponseFormatter to the EndPoint method so a new service object will be received for every request
        //private IResponseFormatter formatter;

        //public WeatherEndpoint(IResponseFormatter responseFormatter)
        //{
        //    formatter = responseFormatter;
        //}

        public async Task Endpoint(HttpContext httpContext, IResponseFormatter formatter)
        {
            await formatter.Format(httpContext, "WeatherEndpoint class no longer static. Dependency on IResponseFormatter service through WeatherEndpoint.Endpoint() method not constructor injection: it's cloudy in Milan");
        }
    }
}
