using System;
using Payroll.Common.Enums;
using Payroll.Entities;

namespace Payroll.Service.Interfaces
{
    public interface IAttendanceService
    {
        int CreateWorkSchedule(string employeeCode, AttendanceCode attCode, DateTime datetime);

        int CreateWorkSchedule(string employeeCode, DateTime clockIn, DateTime clockOut);

        int CreateWorkSchedulesByDateRange(DateTime fromDate, DateTime toDate);

        void Save(Attendance attendance);
    }
}
