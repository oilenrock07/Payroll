using System;
using Payroll.Common.Enums;
using Payroll.Entities;
using System.Collections.Generic;
using Payroll.Entities.Enums;

namespace Payroll.Service.Interfaces
{
    public interface IAttendanceService
    {
        void CreateWorkSchedules();

        int CreateWorkSchedule(int employeeId, AttendanceType attCode, DateTime datetime);

        int CreateWorkSchedule(int employeeId, DateTime clockIn, DateTime clockOut);

        int CreateWorkSchedules(IList<AttendanceLog> logs);

        IList<Attendance> GetAttendanceByDateRange(int employeeId, DateTime fromDate, DateTime toDate);

        IList<Attendance> GetAttendanceByDate(int employeeId, DateTime date);

        IList<Attendance> GetAttendanceForProcessing(int employeeId, DateTime date);

        void Update(Attendance attendance);
    }
}
