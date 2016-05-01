using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Payroll.Common.Enums;
using Payroll.LoginDisplay.Hubs;

namespace Payroll.LoginDisplay.Controllers
{
    public class PayrollApiController : ApiController
    {
        public void Get(int id, string ipAddress, AttendanceCode attCode, string timeInOut)
        {
            var connection = PayrollHub.Connections.FirstOrDefault(x => x.IpAddress == ipAddress);
            if (connection != null)
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<PayrollHub>();
                context.Clients.Client(connection.ConnectionId).broadcastMessage(id, attCode, timeInOut);
            }    
        }
    }
}