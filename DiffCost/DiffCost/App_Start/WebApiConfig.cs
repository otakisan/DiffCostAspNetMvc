using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DiffCost
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "ProjectsApi",
            //    routeTemplate: "api/{controller}/download",
            //    defaults: new { action = "GetCsv" }
            //);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

        }
    }
}
