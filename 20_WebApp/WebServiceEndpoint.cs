using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;                // for StatusCodes
using Microsoft.Extensions.DependencyInjection; // needed for context.RequestServices.GetService<>()
using System.Text.Json;
using WebApp.Models;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebServiceEndpoint
    {
        private static string BASEURL = "api/products";

        public static void MapWebService(this IEndpointRouteBuilder app)
        {
            app.MapGet($"{BASEURL}/{{id}}", async context =>
            {
                long key = long.Parse(context.Request.RouteValues["id"] as string);
                DataContext data = context.RequestServices.GetService<DataContext>();
                Product p = data.Products.Find(key);
                if (p == null)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                }
                else
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize<Product>(p));
                }
            });

            app.MapGet(BASEURL, async context =>
            {
                DataContext data = context.RequestServices.GetService<DataContext>();
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize<IEnumerable<Product>>(data.Products));
            });

            // Cannot invoke through browser for POST, but can be invoked from Windows powershell using Invoke-RestMethod cmdlet as follows:
            // Invoke-RestMethod http://localhost:5000/api/products -Method POST -Body (@{ Name="Swimming goggles";Price=12.75; CategoryId=1; SupplierId=1} | ConvertTo-Json) -ContentType "application/json"
            app.MapPost(BASEURL, async context => {
                DataContext data = context.RequestServices.GetService<DataContext>();
                Product p = await JsonSerializer.DeserializeAsync<Product>(context.Request.Body);
                await data.AddAsync(p);
                await data.SaveChangesAsync();
                context.Response.StatusCode = StatusCodes.Status200OK; //StatusCodes.Status201Created would be better??
            });


        }
    }
}
