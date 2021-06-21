using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApp.Models;

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

            services.AddDistributedMemoryCache();                               // p528 required for TempData in CubedController and view
            services.AddSession(opts => opts.Cookie.IsEssential = true);        // p528

            services.Configure<RazorPagesOptions>(opts => {
                opts.Conventions.AddPageRoute("/Index","/extra/page/{id:long?}");
            });
            services.AddSingleton<CitiesData>();                                // p587

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataContext dataContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseSession();
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
