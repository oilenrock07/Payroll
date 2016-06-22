using System;
using System.Collections.Generic;
using System.Linq;
using Payroll.Entities;
using Payroll.Repository.Interface;
using Payroll.Service.Interfaces;

namespace Payroll.Service.Implementations
{
    public class SchedulerLogService : ISchedulerLogService
    {
        private readonly ISchedulerLogRepository _schedulerLogRepository;

        public SchedulerLogService(ISchedulerLogRepository schedulerLogRepository)
        {
            _schedulerLogRepository = schedulerLogRepository;
        }

        public IEnumerable<SchedulerLog> GetSchedulerLogs(DateTime startDate, DateTime endDate)
        {
            return _schedulerLogRepository.Find(x => x.CreateDate >= startDate && x.CreateDate <= endDate).ToList();
        }
    }
}
