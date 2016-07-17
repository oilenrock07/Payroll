using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Payroll.Common.Enums;
using Payroll.Entities.Enums;
using Payroll.LoginDisplay.Hubs;
using Payroll.Repository.Interface;

namespace Payroll.LoginDisplay.Controllers
{
    public class PayrollApiController : ApiController
    {
        private readonly ILoginDisplayClientRepository _loginDisplayClientRepository;

        public PayrollApiController(ILoginDisplayClientRepository loginDisplayClientRepository)
        {
            _loginDisplayClientRepository = loginDisplayClientRepository;
        }

        public void Get(int id, string ipAddress, AttendanceType attCode, string timeInOut)
        {
            var connection = _loginDisplayClientRepository.Find(x => x.IpAddress == ipAddress).FirstOrDefault();
            if (connection != null)
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<PayrollHub>();
                context.Clients.Client(connection.ClientId).broadcastMessage(id, attCode, timeInOut);
            }
            else
            {

            } 
        }
    }
}