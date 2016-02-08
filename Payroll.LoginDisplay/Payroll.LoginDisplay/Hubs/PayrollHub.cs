using Microsoft.AspNet.SignalR;

namespace Payroll.LoginDisplay.Hubs
{
    public class PayrollHub : Hub
    {
        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }
    }
}