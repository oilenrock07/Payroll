using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
using Payroll.Service.Implementations;
using Payroll.Service.Interfaces;

namespace Payroll.Test.Service
{
    [TestClass]
    public class SchedulerLogsService
    {

        private ISchedulerLogService _schedulerLogService;

        public void Initialize()
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();
            var unitOfWork = new UnitOfWork(databaseFactory);

            ISchedulerLogRepository _schedulerLogRepository = new SchedulerLogRepository(databaseFactory);
            _schedulerLogService = new SchedulerLogService(_schedulerLogRepository);

        }

        [TestMethod]
        public void GetSchedulerLogsByDateTest()
        {
            Initialize();
            var logs = _schedulerLogService.GetSchedulerLogs(DateTime.Now.AddDays(-30), DateTime.Now);
            Assert.IsNotNull(logs);
        }
    }
}
