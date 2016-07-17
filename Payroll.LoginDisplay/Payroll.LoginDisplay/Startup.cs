using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using Payroll.LoginDisplay.Hubs;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Repositories;

[assembly: OwinStartup(typeof(Payroll.LoginDisplay.Startup))]

namespace Payroll.LoginDisplay
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            var databaseFactory = new DatabaseFactory();
            var unitOfWork = new UnitOfWork(databaseFactory);
            var loginDisplayClientRepository = new LoginDisplayClientRepository(databaseFactory);

            GlobalHost.DependencyResolver.Register(typeof(PayrollHub), () => new PayrollHub(unitOfWork, loginDisplayClientRepository));

            app.MapSignalR();
        }
    }
}
