using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Infrastructure.Implementations;
using System.Collections.Generic;
using System;

namespace Payroll.Repository.Repositories
{
    public class LogRepository : Repository<Log>, ILogRepository
    {
        public LogRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
            DbSet = databaseFactory.GetContext().Logs;
        }

        public virtual IEnumerable<Log> GetLogsByDate(DateTime startDate, DateTime endDate)
        {
            return Find(x => startDate <= x.DateLogged && endDate >= x.DateLogged);
        }
    }
}
