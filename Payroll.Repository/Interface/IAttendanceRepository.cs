using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;

namespace Payroll.Repository.Interface
{
    public interface IAttendanceRepository : IRepository<Attendance>
    {
        Attendance GetLastAttendance(string employeeCode);

        IList<Attendance> GetAttendanceByDateRange(string employeeCode, DateTime fromDate, DateTime toDate);
    }
}
