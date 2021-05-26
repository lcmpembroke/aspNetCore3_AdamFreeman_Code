using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Platform
{
    public class Middleware
    {
        private RequestDelegate next;

        public Middleware()
        {
            // deliberately do nothing so next is null and adds terminal support into this middleware see page 273 BOOK
        }
        public Middleware(RequestDelegate nextRequestDelegate)
        {
            next = nextRequestDelegate;
        }

        // must have Invoke() method!!!
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method == HttpMethods.Get
                    && context.Request.Query["custom"] == "true")
            {
                await context.Response.WriteAsync("Class based middleware applied \n");
            }
            // this middleware componenet will only forward requests when construtor
            // has been provided with a non-null value for the nextDelegate parameter
            if (next != null)
            {
                await next(context);
            }
        }
    }

    //---------------------------

    public class LocationMiddleware
    {
        private RequestDelegate next;
        private MessageOptions messageOptions;

        public LocationMiddleware(RequestDelegate nextRequestDelegate, IOptions<MessageOptions> msgOptions)
        {
            next = nextRequestDelegate;
            messageOptions = msgOptions.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == "/location")
            {
                await context.Response.WriteAsync($"\n{messageOptions.CityName}, {messageOptions.CountryName} \n");
            }
            else
            {
                await next(context);
            }
        }
    }





}
