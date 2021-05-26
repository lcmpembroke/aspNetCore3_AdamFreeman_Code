using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Platform.Services;
using Platform.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace Platform
{
    public class Startup
    {
        private IConfiguration Configuration { get; set; }

        public Startup(IConfiguration config)
        {
            Configuration = config;
        }
       
        public void ConfigureServices(IServiceCollection services){
            
            // p408: using in-memory cache that doesn't persist
            //services.AddDistributedMemoryCache(opts => { opts.SizeLimit = 200; });
            
            // p411 - using persistent data cache
            services.AddDistributedSqlServerCache(opts => {
                opts.ConnectionString = Configuration["ConnectionStrings:CacheConnection"];
                opts.SchemaName = "dbo";
                opts.TableName = "DataCache";
            });
            services.AddResponseCaching();  // p413: set up  service used by cache - used only in certain cirumstances
            services.AddSingleton<IResponseFormatter, HtmlResponseFormatter>();

            services.AddDbContext<CalculationContext>(opts => {
                opts.UseSqlServer(Configuration["ConnectionStrings:CalcConnection"]);
            });

            services.AddTransient<SeedData>();

        }


        public void Configure(IApplicationBuilder app, IHostApplicationLifetime lifetime, IWebHostEnvironment env, SeedData seedData)
        {
            app.UseDeveloperExceptionPage();
            app.UseResponseCaching();   // p413: middleware component added before anythings that needs responses cached
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints => {

                // MapEndpoint defined in  Platform.Services.EndpointExtensions.cs
                endpoints.MapEndpoint<SumEndpoint>("/sum/{count:int=1000000000}");

                endpoints.MapGet("/", async context => {
                    await context.Response.WriteAsync("Hello world...");
                });

            });

            bool cmdLineInit = (Configuration["INITDB"] ?? "false") == "true";
            if (env.IsDevelopment() || cmdLineInit)
            {
                seedData.SeedDatabase();
                if (cmdLineInit)
                {
                    lifetime.StopApplication();
                }
            }

        }
    }
}
