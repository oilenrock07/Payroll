using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using Payroll.LoginDisplay.Models.Payroll;

namespace Payroll.LoginDisplay.Hubs
{
    public class PayrollHub : Hub
    {
        public static readonly List<ConnectionDetails> Connections = new List<ConnectionDetails>();

        public void Connect(string ipAddress)
        {
            var connection = Connections.FirstOrDefault(x => x.IpAddress == ipAddress);
            if (connection != null)
            {
                //replace the existing connectionId
                Connections.Remove(connection);
            }

            var connectionId = Context.ConnectionId;
            Connections.Add(new ConnectionDetails
            {
                ConnectionId = connectionId,
                IpAddress = ipAddress
            });

            //Show the timer div
            Clients.Caller.onConnected();
        }
    }
}