using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            try
            {
                // Launch WebApiConfig.cs
                GlobalConfiguration.Configure(WebApiConfig.Register);
            }
            catch (Exception ex)
            { HttpContext.Current.Response.Redirect("index.html?Message=" + ex.Message); }
        }
    }
}
