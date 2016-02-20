using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Payroll.LoginDisplay
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Payroll",
                url: "{Payroll}/{DisplayTimeInOut}/{id}/{attCode}/{timeInOut}",
                defaults: new { controller = "Payroll", action = "DisplayTimeInOut" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Payroll", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
