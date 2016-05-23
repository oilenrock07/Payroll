using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;

namespace Payroll.Repository.Interface
{
    public interface IAttendanceRepository : IRepository<Attendance>
    {
        Attendance GetLastAttendance(int employeeId);

        IList<Attendance> GetAttendanceByDateRange(DateTime fromDate, DateTime toDate);
        IList<Attendance> GetAttendanceByDateRange(int employeeId, DateTime fromDate, DateTime toDate);
    }
}
