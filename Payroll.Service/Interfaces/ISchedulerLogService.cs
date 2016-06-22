using System;
using System.Collections.Generic;
using Payroll.Entities;

namespace Payroll.Service.Interfaces
{
    public interface ISchedulerLogService
    {
        IEnumerable<SchedulerLog> GetSchedulerLogs(DateTime startDate, DateTime endDate);
    }
}
