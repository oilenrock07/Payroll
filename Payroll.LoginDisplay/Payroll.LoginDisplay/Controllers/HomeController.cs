using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Payroll.LoginDisplay.Hubs;

namespace Payroll.LoginDisplay.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public void Send()
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<PayrollHub>();
            context.Clients.All.addNewMessageToPage(1, 1,"08-02-2016");
        }
    }
}
