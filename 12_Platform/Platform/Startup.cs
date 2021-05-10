using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Platform.Services;
using System;
using System.Linq;
using System.Collections.Generic;

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

            // Page 337 - using Service Factory Functions
            //services.AddScoped<IResponseFormatter>( serviceProvider => {
            //    string typeName = Configuration["services.IResponseFormatter"];
            //    return (IResponseFormatter)ActivatorUtilities.CreateInstance(serviceProvider,
            //        typeName == null ? typeof(GuidService) : Type.GetType(typeName, true));
            //});

            //services.AddSingleton<IResponseFormatter, GuidService>();  // singleton - same service object is shared throughout the duration of the application i.e. maintains state

            services.AddScoped<ITimeStamper, DefaultTimeStamper>();         // both interface and its implementation here are defined in TimeStamping.cs file

            // BOOK page 338ff - three services registered,for IResponseFormatter interface with diff implementation classes
            // If the service consumer requests it via context.RequestServices.GetService<IResponseFormatter>();  then the service is resolved using the
            // most recently registered implementation (here it would be GuidService as it's added last)
            services.AddScoped<IResponseFormatter, TextResponseFormatter>();
            services.AddScoped<IResponseFormatter, HtmlResponseFormatter>();
            services.AddScoped<IResponseFormatter, GuidService>();


            services.AddSingleton(typeof(ICollection<>), typeof(List<>));
                


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
                    await formatter.Format(context, "Middleware FUNCTION USING IResponseFormatter SERVICE from context.RequestServices.GetService<IResponseFormatter>....needed to get from context as it's SCOPED lifecycle");

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

                endpoints.MapGet("/single", async context => {
                    IResponseFormatter formatter = context.RequestServices.GetService<IResponseFormatter>();
                    await formatter.Format(context,"Single Service");
                });

                // here service consumer is aware there are multiple implementations of IResponseFormatter interface
                endpoints.MapGet("/multiple", async context => {
                    IResponseFormatter formatter = context.RequestServices.GetServices<IResponseFormatter>().First(f => f.RichOutput);
                    await formatter.Format(context, "Multiple Service");
                });

                // using unbound types in Services
                endpoints.MapGet("/string", async context => {
                    ICollection<string> collection = context.RequestServices.GetService<ICollection<string>>();
                    collection.Add($"Request: { DateTime.Now.ToLongTimeString() }");
                    foreach (string stringItem in collection)
                    {
                        await context.Response.WriteAsync($"String: {stringItem} \n");
                    }
                });

                endpoints.MapGet("/int", async context => {
                    ICollection<int> collection = context.RequestServices.GetService<ICollection<int>>();
                    collection.Add(collection.Count() + 1);
                    foreach (int intItem in collection)
                    {
                        await context.Response.WriteAsync($"Int: {intItem} \n");
                    }
                });

            });

        }
    }
}
