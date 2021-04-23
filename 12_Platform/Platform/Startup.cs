using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options; // for options patter to configure middleware

namespace Platform
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MessageOptions>(options => {
                options.CityName = "Leeds";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // BOOK page 277-278
            app.UseMiddleware<LocationMiddleware>();


            // modifying the HTTPResponse AFTER the next() function has been called
            app.Use(async (context, next) => {
                await next();
                await context.Response.WriteAsync($"\nStatus code: {context.Response.StatusCode} \n");
            });

            // short circuiting the request pipeline...
            app.Use(async (context, next) => {

                if (context.Request.Path == "/short")
                {
                    await context.Response.WriteAsync($"\nRequest was short circuited \n");
                }
                else
                {
                    await next();
                }
            });


            // adding custom middleware to the application before the next() function has been called
            app.Use(async (context, next) => {
                if (context.Request.Method == HttpMethods.Get && context.Request.Query["custom"] == "true")
                {
                    await context.Response.WriteAsync("Custom middleware applied from Startup class directly \n");
                }
                await next();
            });

            app.Map("/branch", branch => {
                branch.UseMiddleware<Middleware>();
                branch.Use(async (context, next) => {
                    await context.Response.WriteAsync("Branch middleware based on given request path... ");
                });
            });

            // adding user-defined class-based middleware to the application
            app.UseMiddleware<Middleware>();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
