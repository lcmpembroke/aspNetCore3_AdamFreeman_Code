using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApp.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;
using WebApp.TagHelpers;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionStrings:ProductConnection"]);
                options.EnableSensitiveDataLogging(true);
            });

            services.AddControllersWithViews().AddRazorRuntimeCompilation();    // P498
            services.AddRazorPages().AddRazorRuntimeCompilation();              // p560
            services.AddSingleton<CitiesData>();                                // p587
            //services.AddTransient<ITagHelperComponent, TimeTagHelperComponent>();   // p640 (commented out for chapter 26)
            //services.AddTransient<ITagHelperComponent, TableFooterTagHelperComponent>();   // p642(commented out for chapter 26)

            services.Configure<AntiforgeryOptions>(opt => opt.HeaderName = "X-XSRF-TOKEN"); //p699
            services.Configure<MvcOptions>(opts => opts.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(value => "Please enter a value"));    // p750

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataContext dataContext, IAntiforgery antiforgery)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();

            // p700 adding custome middleware to set the XSRF-TOKEN cookie - Http only must be set to false so browser allows Javascript code to read the cookie
            app.Use(async (context, next) =>
            {
                if (!context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.Cookies.Append("XSRF-TOKEN", antiforgery.GetAndStoreTokens(context).RequestToken, new CookieOptions { HttpOnly = false });
                }
                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();     // defines routes that will allow controllers to handle requests
                endpoints.MapControllerRoute("forms", "controllers/{controller=Home}/{action=Index}/{id?}");  // page 670
                endpoints.MapDefaultControllerRoute(); // sets up convention based routing
                endpoints.MapRazorPages();
            });

            SeedData.SeedDatabase(dataContext);
        }
    }
}
