using System;
using Payroll.Common.Enums;
using Payroll.Entities;
using System.Collections.Generic;

namespace Payroll.Service.Interfaces
{
    public interface IAttendanceService
    {
        int CreateWorkSchedule(int employeeId, AttendanceCode attCode, DateTime datetime);

        int CreateWorkSchedule(int employeeId, DateTime clockIn, DateTime clockOut);

        int CreateWorkSchedulesByDateRange(DateTime fromDate, DateTime toDate);

        void Add(Attendance attendance);

        void Update(Attendance attendance);

        IList<Attendance> GetAttendanceByDateRange(int employeeId, DateTime fromDate, DateTime toDate);

        IList<Attendance> GetAttendanceByDate(int employeeId, DateTime date);
    }
}
