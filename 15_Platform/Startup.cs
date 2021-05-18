using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;

namespace Platform
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MessageOptions>(Configuration.GetSection("Location"));    // Location section has "CityName": "Buffalo"
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions { FileProvider = new PhysicalFileProvider($"{env.ContentRootPath}/staticfiles"), RequestPath = "/files" });

            app.UseRouting();

            app.UseMiddleware<LocationMiddleware>();    // see Middleware.cs (multiple classes defined in it)

            app.Use(async (context, next) => {
                if (context.Request.Path == "/config")
                {
                    string defaultDebug = Configuration["Logging:LogLevel:Default"];
                    await context.Response.WriteAsync($"The config setting is: {defaultDebug}");

                    string env = Configuration["ASPNETCORE_ENVIRONMENT"];
                    await context.Response.WriteAsync($"\nThe env setting is: {env}");

                    string wsID = Configuration["WebService:Id"];
                    string wsKey = Configuration["WebService:Key"];
                    await context.Response.WriteAsync($"\nThe secret WebService:Id is: {wsID}");
                    await context.Response.WriteAsync($"\nThe secret WebService:Key is: {wsKey}");
                }
                else
                {
                    await next();
                }
            });

            app.UseEndpoints(endpoints => {

                endpoints.MapGet("/", async context => {
                    logger.LogDebug("Response for / started");
                    await context.Response.WriteAsync("Hello World");
                    logger.LogDebug("Response for / completed");
                });

            });

        }
    }
}
