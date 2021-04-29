using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;

namespace Platform.Services
{
    public static class EndpointExensions
    {
        // generic type parameter specifies the endpoint class that will be used
        // the other arguments are to create the route, and name of the endpoint class method that processes the requests
        public static void MapEndpoint<T>(this IEndpointRouteBuilder app, string path, string methodName = "Endpoint")
        {
            MethodInfo methodInfo = typeof(T).GetMethod(methodName);
            if (methodInfo == null || methodInfo.ReturnType != typeof(Task))
            {
                throw new System.Exception("Method cannot be used");
            }
            // new instance of endpoint class created   **NOTE The ActivatorUtilities class is in the DependencyInjection namespace.
            // It provides methods for instantiating classes that have dependencies declared through their constructor
            T endpointInstance = ActivatorUtilities.CreateInstance<T>(app.ServiceProvider);

            // a delegate to the specified method is used to create a route
            //app.MapGet(path, (RequestDelegate)methodInfo.CreateDelegate(typeof(RequestDelegate), endpointInstance));

            ParameterInfo[] methodParams = methodInfo.GetParameters();
            app.MapGet(path, 
                context => (Task)methodInfo.Invoke(endpointInstance, 
                                                    methodParams.Select(p => p.ParameterType == typeof(HttpContext) ? context : app.ServiceProvider.GetService(p.ParameterType))
                                                    .ToArray())
            );

        }
    }
}
