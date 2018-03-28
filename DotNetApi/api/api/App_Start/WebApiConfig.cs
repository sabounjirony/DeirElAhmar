using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Routing;
using Api.Filters;

namespace Api
{
    public static class WebApiConfig
    {
        public static string ORIGINS = "*";
        public static string HEADERS = "*";
        public static string METHODS = "*";

        public static void Register(HttpConfiguration config)
        {
            var constraints = new { httpMethod = new HttpMethodConstraint(HttpMethod.Options) };
            config.Routes.IgnoreRoute("OPTIONS", "{controller}/{action}", constraints);

            // Enable CORS
            var cors = new EnableCorsAttribute(ORIGINS, HEADERS, METHODS);
            config.EnableCors(cors);
            //config.EnableCors();

            // Add handler to deal with preflight requests, this is the important part
            //config.MessageHandlers.Add(new PreflightRequestsHandler()); // Defined above

            // Web API routes
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(name: "Default", routeTemplate: "{controller}");
            config.Routes.MapHttpRoute(name: "NoId", routeTemplate: "{controller}/{action}");
            config.Routes.MapHttpRoute(name: "WithId", routeTemplate: "{controller}/{action}/{id}");
            config.Routes.MapHttpRoute(name: "TwoString", routeTemplate: "{controller}/{action}/{id}/{code}");

            GlobalConfiguration.Configuration.Filters.Add(new CustomAuthorizationFilter());
            GlobalConfiguration.Configuration.Formatters.Clear();
            GlobalConfiguration.Configuration.Formatters.Add(new JsonMediaTypeFormatter());
            BL.BlCommon.InitiateService();

        }
    }
}
