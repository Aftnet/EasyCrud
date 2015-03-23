using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using EasyCrud.Logging;
using EasyCrud.WebAPI.Exceptions;
using EasyCrud.WebAPI.Formatters;

namespace EasyCrud.WebAPI.Configuration
{
    public class WebApiConfig
    {
        protected readonly IAuthenticationFilter AuthenticationFilter;
        protected readonly ILogger Logger;

        public WebApiConfig(IAuthenticationFilter authenticationFilter, ILogger logger)
        {
            AuthenticationFilter = authenticationFilter;
            Logger = logger;
        }

        public void AppStart()
        {
            GlobalConfiguration.Configure(Register);
            // Add a reference here to the new MediaTypeFormatter that adds text/plain support
            GlobalConfiguration.Configuration.Formatters.Insert(0, new TextMediaTypeFormatter());
            GlobalConfiguration.Configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            var exceptionHandler = new GlobalExceptionHandler(Logger);
            GlobalConfiguration.Configuration.Services.Replace(typeof(IExceptionHandler), exceptionHandler);
        }

        public void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Filters.Add(AuthenticationFilter);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));
        }
    }
}
