using System;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Payroll.Common.Enums;
using Payroll.LoginDisplay.Hubs;

namespace Payroll.LoginDisplay.Controllers
{
    public class PayrollApiController : ApiController
    {
        //timeinout should be converted to sqldate format (1989-10-30)
        public void Get(int id, AttendanceCode attCode, string timeInOut)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<PayrollHub>();
            context.Clients.All.addNewMessageToPage(id, attCode, timeInOut);
        }
    }
}