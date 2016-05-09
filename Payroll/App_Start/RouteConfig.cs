using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Payroll
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "AttendanceRoute",
            //    url: "{controller}/{action}/{code}",
            //    defaults: new { controller = "Attendance", action = "ClockIn" }
            //    );

            //Fix route for employee
            //routes.MapRoute(
            //    name: "EmployeeListRoute",
            //    url: "{controller}/{action}/{Page}",
            //    defaults: new { controller = "Home", action = "Index" }
            //);

            routes.MapRoute(
                name: "EmplloyeeLeaveRoute",
                url: "EmployeeController/{action}/{month}/{year}",
                defaults: new { controller = "Employee", action = "EmployeeLeaves", month = UrlParameter.Optional, year = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "EmployeeRoute",
                url: "EmployeeController/{action}/{id}",
                defaults: new { controller = "Employee", action = "Index", id = UrlParameter.Optional }
            );



            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


        }
    }
}
