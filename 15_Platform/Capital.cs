
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Platform
{
    public static class Capital
    {
        // note now static method to fit in with the way endpoints are used in routes
        public static async Task Endpoint(HttpContext context)
        {
            string capital = null;
            // use route data to get segment variable named country through the indexer defined
            // by the RouteValuesDictioinary (the return type of context.Request.RouteValues )
            // The indexer returns an object that's cast to a string
            string country = context.Request.RouteValues["country"] as string;
            switch ((country ?? "").ToLower())
            {
                case "uk":
                    capital = "London";
                    break;
                case "france":
                    capital = "Paris";
                    break;
                case "monaco":
                    // context.Response.Redirect($"/population/{country}");
                    // replace direct dependence on the population URL, to use the ROUTING feature instead
                    // NB the Link Generator HAS to be obtained using DI
                    LinkGenerator generator = context.RequestServices.GetService<LinkGenerator>();
                    string url = generator.GetPathByRouteValues(context, "population", new { city = country });
                    context.Response.Redirect(url);
                    return;
            }
            if (capital != null)
            {
                await context.Response
                    .WriteAsync($"{capital} is the capital of {country}");
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
            }
        }
    }
}