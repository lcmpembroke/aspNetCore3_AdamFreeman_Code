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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseMiddleware<Population>();
            //app.UseMiddleware<Capital>();

            app.UseRouting();

            app.UseEndpoints(endpoints => {
                endpoints.MapGet("routing", async context => { await context.Response.WriteAsync("Request was Routed"); });
                endpoints.MapGet("capital/uk", new Capital().Invoke);
                endpoints.MapGet("population/paris", new Population().Invoke);
                endpoints.MapGet("{first}/{second}/{third}", async context => 
                {
                    await context.Response.WriteAsync("\nRequest was Routed\n");
                    foreach (var item in context.Request.RouteValues)
                    {
                        await context.Response.WriteAsync($"{item.Key}, {item.Value}\n");
                    }
                });
            });

            app.Use(async (context, next) => {
                await context.Response.WriteAsync("Terminal Middleware Reached");
            });

        }
    }
}
