using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Routing;
using Platform.Services;

namespace Platform
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services){
            services.AddSingleton<IResponseFormatter, HtmlResponseFormatter>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IResponseFormatter formatter)
        {
            app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UseMiddleware<WeatherMiddleware>();

            app.Use(async (context,next) => {
                if (context.Request.Path == "/middleware/function")
                {
                    //await TextResponseFormatter.Singleton.Format(context, "Middleware FUNCTION: it's snowing in Chicago");
                    //await TypeBroker.Formatter.Format(context, "Middleware FUNCTION: it's snowing in Chicago");
                    await formatter.Format(context, "Middleware FUNCTION USING IResponseFormatter SERVICE: it's snowing in Chicago");
                }
                else
                {
                    await next();
                }
            });


            // **** Routes registered in the UseEndpoints() method
            app.UseEndpoints(endpoints => {

                //endpoints.MapGet("/endpoint/class", WeatherEndpoint.Endpoint);
                endpoints.MapWeather("/endpoint/class");

                endpoints.MapGet("/endpoint/function", async context => {
                    //await TextResponseFormatter.Singleton.Format(context, "Endpoint function: it's sunny in LA");
                    await TypeBroker.Formatter.Format(context, "Endpoint function: it's sunny in LA");
                });
            });

        }
    }
}
