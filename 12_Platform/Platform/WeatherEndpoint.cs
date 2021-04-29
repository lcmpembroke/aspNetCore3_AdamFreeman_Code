using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Platform.Services;
using System.Threading.Tasks;

namespace Platform
{
    public class WeatherEndpoint
    {

        public static async Task Endpoint(HttpContext httpContext, IResponseFormatter responseFormatter)
        {
            // NOTE: drawback of using RequestServices() method is service must be resolved for every request that is routed to the endpoints
            // rather than a single object able to dal with multiple responses
            //IResponseFormatter formatter = httpContext.RequestServices.GetRequiredService<IResponseFormatter>();
            //await formatter.Format(httpContext, "WeatherEndpoint endpoint class USING httpContext.RequestServices.GetRequiredService<IResponseFormatter>()  on the HttpContext object: it's cloudy in Milan");

            // NOTE: here using an Adapter Function - more elegant as get the service when the endpoint's route is created (and not for each request) by declaring
            // a dependency on the IResponseFOrmatter interface
            // See Services/EndpointExtensions.cs  for new static MapWeather() function which
            await responseFormatter.Format(httpContext, "WeatherEndpoint endpoint class USING an Adapter function: it's cloudy in Milan");
        }
    }
}
