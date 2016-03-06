using System;
using System.Collections.Generic;
using System.Linq;
using Payroll.Entities;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;

namespace Payroll.Repository.Repositories
{
    public class AttendanceLogRepository : Repository<AttendanceLog>, IAttendanceLogRepository
    {
        public AttendanceLogRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().AttendanceLog;
        }

        public IList<AttendanceLog> GetAttendanceLogs(DateTime fromDate, DateTime toDate, bool isRecorded)
        {
            return Find(a => a.IsRecorded == isRecorded
                && a.ClockInOut >= fromDate && a.ClockInOut < toDate)
                    .OrderBy(a => a.EmployeeId).ThenBy(a => a.ClockInOut).ToList();
        }

    }
}
