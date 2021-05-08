using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Platform.Services;

namespace Platform
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services){
            //services.AddSingleton<IResponseFormatter, HtmlResponseFormatter>();
            //services.AddTransient<IResponseFormatter, HtmlResponseFormatter>();
            //services.AddScoped<IResponseFormatter, HtmlResponseFormatter>();
            services.AddScoped<IResponseFormatter, TimeResponseFormatter>();
            services.AddScoped<ITimeStamper, DefaultTimeStamper>();         // both interface and its implementation here are defined in TimeStamping.cs file



            //services.AddTransient<IGuidResponseService, GuidService>(); // within a REQUEST a new service is made for every instance i.e. essentially STATELESS
            //services.AddScoped<IGuidResponseService, GuidService>();  // within a REQUEST a service object is shared for different instance
            services.AddSingleton<IGuidResponseService, GuidService>();  // the same service object is shared throughout the duration of the application i.e. maintains state

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseDefaultFiles();          // tells ASP.NETCore to search for index or default files
            app.UseStaticFiles();           // to pick up from wwwroot folder
            app.UseRouting();
            app.UseMiddleware<WeatherMiddleware>();

            app.Use(async (context,next) => {
                if (context.Request.Path == "/middleware/function")
                {
                    //await TextResponseFormatter.Singleton.Format(context, "Middleware FUNCTION: it's snowing in Chicago");
                    //await TypeBroker.Formatter.Format(context, "Middleware FUNCTION: it's snowing in Chicago");

                    //- page 333..the following two lines will give an exception for SCOPED services: ... so need to change to get service from the HttpContext object
                    //IResponseFormatter formatter = app.ApplicationServices.GetService<IResponseFormatter>(); 
                    //await formatter.Format(context, "Middleware FUNCTION USING IResponseFormatter SERVICE from app.ApplicationServices: it's snowing in Chicago");

                    IResponseFormatter formatter = context.RequestServices.GetService<IResponseFormatter>();
                    await formatter.Format(context, "Middleware FUNCTION USING IResponseFormatter SERVICE from HttpContext object..");  // needed for SCOPED services
                    
                    IGuidResponseService guidShow = app.ApplicationServices.GetService<IGuidResponseService>();
                    await guidShow.FormatGuid(context, "Middleware FUNCTION USING IGuidResponseService SERVICE...");
                }
                else
                {
                    await next();
                }
            });


            // **** Routes registered in the UseEndpoints() method
            app.UseEndpoints(endpoints => {

                // page 312
                //endpoints.MapGet("/endpoint/class", WeatherEndpoint.Endpoint);

                // page 322 using adapter function
                //endpoints.MapWeather("/endpoint/class");

                // page 324 using the activation utility class
                endpoints.MapEndpoint<WeatherEndpoint>("/endpoint/class");

                
                endpoints.MapGet("/endpoint/function", async context => {
                    //await TextResponseFormatter.Singleton.Format(context, "Endpoint function: it's sunny in LA");
                    //await TypeBroker.Formatter.Format(context, "Endpoint function: it's sunny in LA");

                    //IResponseFormatter formatter = app.ApplicationServices.GetService<IResponseFormatter>();        REPLACING THIS WITH BELOW PAGE 333
                    //await formatter.Format(context, "Endpoint function using transient IResponseFormatter service from app.ApplicationServices: it's sunny in LA");

                    IResponseFormatter formatter = context.RequestServices.GetService<IResponseFormatter>();
                    await formatter.Format(context, "Endpoint function using transient IResponseFormatter service from CONTEXT object: it's sunny in LA");

                });
            });

        }
    }
}
