using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Platform.Services
{
    public static class EndpointExensions
    {
        public static void MapWeather(this IEndpointRouteBuilder app, string path)
        {
            IResponseFormatter formatter = app.ServiceProvider.GetRequiredService<IResponseFormatter>();

            app.MapGet(path, context => Platform.WeatherEndpoint.Endpoint(context, formatter));
        }
    }
}
