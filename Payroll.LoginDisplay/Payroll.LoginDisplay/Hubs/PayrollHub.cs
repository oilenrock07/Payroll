using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Payroll.LoginDisplay.Models.Payroll;

namespace Payroll.LoginDisplay.Hubs
{
    public class PayrollHub : Hub
    {
        public static readonly List<ConnectionDetails> Connections = new List<ConnectionDetails>();

        public void Connect(string ipAddress)
        {
            var connectionId = Context.ConnectionId;
            Connections.Add(new ConnectionDetails
            {
                ConnectionId = connectionId,
                IpAddress = ipAddress
            });

            //Check if ip address is valid


            //Show the timer div
            Clients.Caller.onConnected();
        }
    }
}