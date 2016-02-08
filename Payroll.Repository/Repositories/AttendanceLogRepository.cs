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

        }

        public IList<AttendanceLog> GetAttendanceLogs(DateTime fromDate, DateTime toDate, Boolean isRecorded)
        {
            return Find(a => !a.IsRecorded
                && a.ClockInOut >= fromDate && a.ClockInOut <= toDate)
                    .OrderBy(a => a.EmployeeCode).ThenBy(a => a.ClockInOut).ToList();
        }

    }
}
