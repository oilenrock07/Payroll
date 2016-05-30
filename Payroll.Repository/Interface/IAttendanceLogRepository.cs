using Payroll.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using Payroll.Entities;
using Payroll.Repository.Models;

namespace Payroll.Repository.Interface
{
    public interface IAttendanceLogRepository : IRepository<AttendanceLog>
    {
        IList<AttendanceLog> GetAttendanceLogs(DateTime fromDate, DateTime toDate, bool isRecorded);
        IEnumerable<AttendanceLogDao> GetAttendanceLogsWithName(DateTime fromDate, DateTime toDate);
        IList<AttendanceLog> GetAttendanceLogs(bool isRecorded);
    }
}
