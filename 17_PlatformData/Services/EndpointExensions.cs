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
        // BOOK p330 - 334
        // Big point to note about this extension method is that it's set up to avoid requesting a scoped serice OUTSIDE of the scope which would
        // cause a "cannot resolved scoped service from root provider" EXCEPTION
        // HOW? by accessing the scoped service through the HttpContext object rather than DIRECTLY through the IServiceProvider
        //          THe HttpContext class defines a RequestServices property which returns an IServiceProvider object which allows access to scoped services
        // NOTE this is only for services where dependency is declared through METHOD parameter, NOT through CONSTRUCTOR injection - that way endpoints
        // don't use scoped services inappropriately.

        // so Scoped Services MUST be accessed through the HttpContext object


        // generic type parameter specifies the endpoint class that will be used
        // the other arguments are to create the route, and name of the endpoint class method that processes the requests
        public static void MapEndpoint<T>(this IEndpointRouteBuilder app, string path, string methodName = "Endpoint")
        {
            MethodInfo methodInfo = typeof(T).GetMethod(methodName);
            if (methodInfo == null || methodInfo.ReturnType != typeof(Task))
            {
                throw new System.Exception("Method cannot be used");
            }

            //COMMENTING OUT BELOW AS NOW GOING TO CREATE NEW HANDLERS FOR EACH REQUESTS TO REMOVE THE NEED FOR THE ENDPOINT CLASS
            // TO KNOW THE SERVICE LIFECYCLE (i.e. both constructor and method dependencies can be resolved without worrying about which services are scoped which
            // would cause an exception if requested out of scope

            // new instance of endpoint class created   **NOTE The ActivatorUtilities class is in the DependencyInjection namespace.
            // It provides methods for instantiating classes that have dependencies declared through their CONSTRUCTOR
            //T endpointInstance = ActivatorUtilities.CreateInstance<T>(app.ServiceProvider);
            //ParameterInfo[] methodParams = methodInfo.GetParameters();
            //app.MapGet(path, 
            //    context => (Task)methodInfo.Invoke(endpointInstance, 
            //                                        methodParams.Select(p => p.ParameterType == typeof(HttpContext) ? context : context.RequestServices.GetService(p.ParameterType))
            //                                        .ToArray())
            //);


            // BOOK page 332: creating new handlers for each request - so a new instace of the endpoint class handles each request, and don't need to know service lifecycle
            ParameterInfo[] methodParams = methodInfo.GetParameters();
            app.MapGet(path, context => {
                T endpointInstance = ActivatorUtilities.CreateInstance<T>(context.RequestServices);
                return (Task)methodInfo.Invoke(endpointInstance,
                                methodParams.Select(p =>
                                                    p.ParameterType == typeof(HttpContext) ? context : context.RequestServices.GetService(p.ParameterType)).ToArray());
            });
        }
    }
}
