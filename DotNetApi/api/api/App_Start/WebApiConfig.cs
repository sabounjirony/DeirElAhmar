using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Routing;

namespace api
{
    public static class WebApiConfig
    {
        public static string ORIGINS = "*";
        public static string HEADERS = "*";
        public static string METHODS = "*";

        public static void Register(HttpConfiguration config)
        {
            //var json = config.Formatters.JsonFormatter;
            //json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            //config.Formatters.Remove(config.Formatters.XmlFormatter);

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

        }
    }
}
