using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models; // Swashbuckle for OpenAPI sec implementation

namespace WebApp
{
    public class Startup
    {
        private IConfiguration Configuration { get; set; }
        public Startup(IConfiguration config)
        {
            Configuration = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(options => {
                options.UseSqlServer(Configuration["ConnectionStrings:ProductConnection"]);
                options.EnableSensitiveDataLogging(true);
            });

            //services.AddControllers();  // defines services required by MVC framework
            //services.Configure<JsonOptions>(opts => { opts.JsonSerializerOptions.IgnoreNullValues = true; });  // AFFECTS ALL JSON responses - only use with caution
            
            // p475, P479 for addition of XML serializer formatter
            services.AddControllers().AddNewtonsoftJson().AddXmlSerializerFormatters();
            services.Configure<MvcNewtonsoftJsonOptions>(opts => {
                opts.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });

            services.Configure<MvcOptions>(opts => { 
                opts.RespectBrowserAcceptHeader = true;     // disables the fallback to JSON when accept header contains */*  see p481
                opts.ReturnHttpNotAcceptable = true;        // disables the fallback to JSON when the client requests an unsupported data format
            });

            services.AddSwaggerGen(opts =>
            {
                opts.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApp", Version = "v1" });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataContext dataContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseMiddleware<TestMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });

                // p439 web service using custom endpoint
                //endpoints.MapWebService();

                // p442 web service using controller
                endpoints.MapControllers();     // defines routes that will allow controllers to handle requests
            });

            app.UseSwagger();
            app.UseSwaggerUI(opts =>
            {
                opts.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApp");
            });

            SeedData.SeedDatabase(dataContext);
        }
    }
}
