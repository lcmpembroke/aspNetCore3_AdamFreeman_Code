using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Routing;

namespace Platform
{
    public class Startup_chapter13_work
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RouteOptions>(options => { options.ConstraintMap.Add("countryName", typeof(CountryRouteConstraint)); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // **** Route SELECTION happens in UseRouting() method
            app.UseRouting();


            // *** middleware added between UseRouting() and UseEndpoints() can see which endpoint has been selected before
            // the response is generated and alter its behaviour
            // chapter 13 p307
            app.Use(async (context,next) => {
                Endpoint end = context.GetEndpoint();
                if (end != null)
                {
                    await context.Response.WriteAsync($"{end.DisplayName} selected \n");
                }
                else
                {
                    await context.Response.WriteAsync($"No endpoint selected \n");
                }
                await next();
            });


            // **** Routes registered in the UseEndpoints() method
            app.UseEndpoints(endpoints => {
                endpoints.MapGet("routing", async context => { await context.Response.WriteAsync("Request was Routed"); });

                // .Add(...) breaks route ambiguity by defining order relative to other potentially matching routes (awkward)
                // precedence given to lowest Order value
                endpoints.MapGet("{number:int}", async context =>
                {
                    await context.Response.WriteAsync("\nRouted to the int endpoint\n");
                })
                .WithDisplayName("Int endpoint")
                .Add(b => ((RouteEndpointBuilder)b).Order = 1);

                endpoints.MapGet("{number:double}", async context =>
                {
                    await context.Response.WriteAsync("\nRouted to the double endpoint\n");
                })
                .WithDisplayName("Double endpoint")
                .Add(b => ((RouteEndpointBuilder)b).Order = 2);


                endpoints.MapGet("{first:int}/{second:bool}/{*catchall}", async context => 
                {
                    await context.Response.WriteAsync("\nRequest was Routed\n");
                    foreach (var item in context.Request.RouteValues)
                    {
                        await context.Response.WriteAsync($"{item.Key}, {item.Value}\n");
                    }
                });

                endpoints.MapGet("files/{filename}.{ext}", async context =>
                {
                    await context.Response.WriteAsync("\nRequest was Routed\n");
                    foreach (var item in context.Request.RouteValues)
                    {
                        await context.Response.WriteAsync($"{item.Key}, {item.Value}\n");
                    }
                });


                // adds a cutom constraint (defined in ConfigureServices) that checks it against CountryRouteConstraint class
                endpoints.MapGet("capital/{country:countryName}", Capital.Endpoint);

                // assign metadata to the route to give the NAME of the route
                //endpoints.MapGet("population/{city}", Population.Endpoint).WithMetadata(new RouteNameMetadata("population"));
                endpoints.MapGet("size/{city?}", Population.Endpoint).WithMetadata(new RouteNameMetadata("population"));

                // last resort to match any request
                endpoints.MapFallback(async context => {
                    await context.Response.WriteAsync("Routed to fallback endpoint");
                });
            });

            app.Use(async (context, next) => {
                await context.Response.WriteAsync("Terminal Middleware Reached");
            });

        }
    }
}
