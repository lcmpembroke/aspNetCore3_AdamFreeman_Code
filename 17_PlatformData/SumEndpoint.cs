using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Distributed;
using Platform.Models;
using Platform.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Platform
{
    public class SumEndpoint
    {
        // method here is using persistent database context to store previously Calculation object results - note that data retrieved from database
        // is not cached and each request leads to a new SWL query. Depending on frequency and complexity of queries, may want to cache either the values or the responses
        public async Task Endpoint(HttpContext context, CalculationContext dataContext)
        {
            int count = int.Parse((string)context.Request.RouteValues["count"]);
            long total = dataContext.Calculations.FirstOrDefault(c => c.Count == count)?.Result ?? 0;

            if (total == 0)
            {
                for (int i = 1; i <= count; i++)
                {
                    total += i;
                }
                dataContext.Calculations!.Add(new Calculation() { Count = count, Result = total });
                await dataContext.SaveChangesAsync();
            }
            string totalString = $"({DateTime.Now.ToLongTimeString()}) {total}";
            await context.Response.WriteAsync($"{DateTime.Now.ToLongTimeString()} Total for { count } values:\n{totalString}\n");
        }

        // method here is using Response Caching
        public async Task Endpoint_beforeP422(HttpContext context, IDistributedCache cache, IResponseFormatter formatter, LinkGenerator generator)
        {
            int count = int.Parse((string)context.Request.RouteValues["count"]);
            long total = 0;
            for (int i = 1; i <= count; i++)
            {
                total += i;
            }
            string totalString = $"({DateTime.Now.ToLongTimeString()}) {total}";

            // adds header to response - this is the important bit to enable response caching
            // the middleware will only cache responses that have a Cache-Control header that contains a public directive. Note here the max-age sets response to be cached for 2minutes
            context.Response.Headers["Cache-Control"] = "public, max-age=120";

            string url = generator.GetPathByRouteValues(context, null, new { count = count });

            await formatter.Format(context, $"<div>({DateTime.Now.ToLongTimeString()}) Total for { count }"
                + $" values:</div><div>{totalString}</div>"
                + $" <a href={url}>RELOAD</a>"
                );
        }

        // method here is using value Caching
        public async Task Endpoint_beforeResponseCaching(HttpContext context, IDistributedCache cache)
        {
            int count = int.Parse((string)context.Request.RouteValues["count"]);
            string cacheKey = $"sum_{count}";
            string totalString = await cache.GetStringAsync(cacheKey);
            if (totalString == null)
            {
                long total = 0;
                for (int i = 0; i < count; i++)
                {
                    total += i;
                }
                totalString = $"({DateTime.Now.ToLongTimeString()}) {total}";
                await cache.SetStringAsync(cacheKey, totalString, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2) });
            }

            await context.Response.WriteAsync($"{DateTime.Now.ToLongTimeString()} Total for { count } values:\n{totalString}\n");
        }
    }
}
