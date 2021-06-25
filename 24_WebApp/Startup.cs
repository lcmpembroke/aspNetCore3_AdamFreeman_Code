using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApp.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;
using WebApp.TagHelpers;

namespace WebApp
{
    public class Startup
    {
        private IConfiguration Configuration { get; set; }
        public Startup(IConfiguration config)
        {
            Configuration = config;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(options => {
                options.UseSqlServer(Configuration["ConnectionStrings:ProductConnection"]);
                options.EnableSensitiveDataLogging(true);
            });

            services.AddControllersWithViews().AddRazorRuntimeCompilation();    // P498
            services.AddRazorPages().AddRazorRuntimeCompilation();              // p560
            services.AddSingleton<CitiesData>();                                // p587
            //services.AddTransient<ITagHelperComponent, TimeTagHelperComponent>();   // p640 (commented out for chapter 26)
            //services.AddTransient<ITagHelperComponent, TableFooterTagHelperComponent>();   // p642(commented out for chapter 26)

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataContext dataContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();     // defines routes that will allow controllers to handle requests
                endpoints.MapDefaultControllerRoute(); // sets up convention based routing
                endpoints.MapRazorPages();
            });

            SeedData.SeedDatabase(dataContext);
        }
    }
}
