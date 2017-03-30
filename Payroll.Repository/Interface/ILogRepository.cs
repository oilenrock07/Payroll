using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;

namespace Payroll.Repository.Interface
{
    public interface ILogRepository : IRepository<Log>
    {
        IEnumerable<Log> GetLogsByDate(DateTime startDate, DateTime endDate);
    }
}
