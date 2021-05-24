using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.HostFiltering;

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
            // Gets settings from appsettings.json, but no validation done, and any typos won't be picked up
            //services.Configure<MessageOptions>(Configuration.GetSection("Location"));    // Location section has "CityName": "Buffalo"
            
            // aternative approach which can do validation on the configuration settings in appsettings.json:
            services.AddOptions<MessageOptions>().Bind(Configuration.GetSection("Location")).ValidateDataAnnotations();

            // Chapter 16: page 377
            services.Configure<CookiePolicyOptions>(opt => {
                opt.CheckConsentNeeded = context => true;
            });

            // p382 Creates in-memory Cache service and stores session data for a single instance of ASP.NET Core runtime
            // NB it's not actually distributed - if application is scaled by deploying multiple instances, need to use SQLServer or Redis cache
            services.AddDistributedMemoryCache();

            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // time after which session expires
                options.Cookie.IsEssential = true;
            });

            services.AddHsts(opts => {
                opts.MaxAge = TimeSpan.FromDays(1);
                opts.IncludeSubDomains = true;
            });

            services.Configure<HostFilteringOptions>(opts => {
                opts.AllowedHosts.Clear();                  // removes entries from appsettings.json file
                opts.AllowedHosts.Add("*.example.com");     // accept hosts in example.com domain
            });
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger, IOptionsMonitor<MessageOptions> msgOptions)
        {
            //app.UseDeveloperExceptionPage();
            app.UseExceptionHandler("/error.html");
            
            if (env.IsProduction())
            {
                app.UseHsts();
            }
            app.UseStaticFiles();
            app.UseHttpsRedirection();  // Enforce Https

            // P397 only for 400-600 status codes, not for unhandled exceptions and won't alter a response that's already begun ie after another middleware has already started generating a response
            // therefore usually used in conjunction with UseExceptionHandler
            app.UseStatusCodePages("text/html", ResponseStrings.DefaultResponse);   // adds response enriching middleware
            
            app.UseCookiePolicy();
            app.UseMiddleware<ConsentMiddleware>();
            // middleware that on processing a request, retrieves session data from cache and makes it available through HttpContext
            // If no session cookie present, it starts a new session and adds a session cookie to the response so subsequent requests are identified as part of hte session
            app.UseSession();

            app.UseRouting();

            app.Use(async (context, next) => {
                if (context.Request.Path == "/error")
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    // middleware components are required to return a Task, but because there is no work for this middleware to do, 
                    // have just used the Task.CompletedTask property
                    await Task.CompletedTask;   
                }
                else
                {
                    await next();
                }
            });

            //app.Run(context => { throw new Exception("Something has gone wrong"); });


            app.UseEndpoints(endpoints => {

                //Book p375-377
                endpoints.MapGet("/cookie", async context => {
                    int counter1 = int.Parse(context.Request.Cookies["counter1"] ?? "0") + 1;
                    // IsEssential means cookie will be added to responses
                    context.Response.Cookies.Append("counter1",counter1.ToString(), 
                        new CookieOptions { 
                            MaxAge = TimeSpan.FromMinutes(30), 
                            IsEssential = true
                        });

                    int counter2 = int.Parse(context.Request.Cookies["counter2"] ?? "0") + 1;
                    // IsEssential by default is false, so this cookie is not added to responses due to the UseCookiePolicy() middleware in the pipeline
                    context.Response.Cookies.Append("counter2", counter2.ToString(), new CookieOptions { MaxAge = TimeSpan.FromMinutes(30) });

                    await context.Response.WriteAsync($"Counter1: {counter1}, Counter2: {counter2}");
                });



                // BOOK: p383
                endpoints.MapGet("/cookieSession", async context => {

                    // NOTE that session cookie doesn't have to be dealt with (eg loading session data from cache)
                    // it's all done by the session middleware automatically which gives the results in the HttpContext.Session property
                    // THEREFORE shold only attempt to access session data in middleware/endpoints that are in request pipeline AFTER UseSession() method called
                    int counter1 = (context.Session.GetInt32("counter1") ?? 0) + 1;
                    int counter2 = (context.Session.GetInt32("counter2") ?? 0) + 1;

                    context.Session.SetInt32("counter1", counter1);
                    context.Session.SetInt32("counter2", counter2);

                    // optional to use this - but good practice as throws exception if session data can't be stored in cache
                    // defaut  is that no error is reported if there is caching problems --> leads to unpredictable app behaviour
                    await context.Session.CommitAsync();    

                    await context.Response.WriteAsync($"Counter1: {counter1}, Counter2: {counter2}");
                });



                endpoints.MapGet("/messageOptions", async context => {
                    await context.Response.WriteAsync($"Message options value: {msgOptions.CurrentValue.CityName}, {msgOptions.CurrentValue.CountryName}, {msgOptions.CurrentValue.ToString()}");
                });

                endpoints.MapGet("/clear", context => {
                    context.Response.Cookies.Delete("counter1");
                    context.Response.Cookies.Delete("counter2");
                    context.Response.Redirect("/");
                    return Task.CompletedTask;
                });



                endpoints.MapFallback(async context => await context.Response.WriteAsync("Hello World"));

            });

        }
    }
}
