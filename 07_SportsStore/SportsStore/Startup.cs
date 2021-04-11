using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SportsStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SportsStore
{
    public class Startup
    {

        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        private IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddDbContext<StoreDbContext>(opts => {
                opts.UseSqlServer(
                    Configuration["ConnectionStrings:SportsStoreConnection"]);
            });
            services.AddScoped<IStoreRepository, EFStoreRepository>();
            services.AddRazorPages();
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseSession();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // URL example: /Soccer/Page2
                endpoints.MapControllerRoute(
                    "catpage",
                    "{category}/Page{productPage:int}",
                    new { Controller = "Home", action = "Index" });

                // URL example: /Page2      shows specified page for all categories
                endpoints.MapControllerRoute(
                    "page",
                    "Page{productPage:int}",
                    new { Controller = "Home", action = "Index", productPage = 1 });

                // URL example: /Soccer     shows first page of the Soccer category
                endpoints.MapControllerRoute(
                    "category",
                    "{category}",
                    new { Controller = "Home", action = "Index", productPage = 1 });

                // URL example: /Products/Page2
                endpoints.MapControllerRoute(
                    "pagination", 
                    "Products/Page{productPage}",
                    new { Controller = "Home", action = "Index" });
                
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
            SeedData.EnsurePopulated(app);
        }
    }
}
