using System.Linq;
using Microsoft.AspNet.SignalR;
using Payroll.Repository.Interface;
using Payroll.Infrastructure.Interfaces;
using Payroll.Entities;

namespace Payroll.LoginDisplay.Hubs
{
    public class PayrollHub : Hub
    {
        private readonly ILoginDisplayClientRepository _loginDisplayClientRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PayrollHub(IUnitOfWork unitOfWork, ILoginDisplayClientRepository loginDisplayClientRepository)
        {
            _unitOfWork = unitOfWork;
            _loginDisplayClientRepository = loginDisplayClientRepository;
        }

        public void Connect(string ipAddress)
        {
            //remove the existing connection
            var client = _loginDisplayClientRepository.Find(x => x.IpAddress == ipAddress).FirstOrDefault();
            if (client != null)
            {
                _loginDisplayClientRepository.PermanentDelete(client);
                _unitOfWork.Commit();
            }

            var connectionId = Context.ConnectionId;
            var loginDisplayClient = new LogInDisplayClient
            {
                IpAddress = ipAddress,
                ClientId = connectionId
            };

            _loginDisplayClientRepository.Add(loginDisplayClient);
            _unitOfWork.Commit();

            //Show the timer div
            Clients.Caller.onConnected();
        }
    }
}