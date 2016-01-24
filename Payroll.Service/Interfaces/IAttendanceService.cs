using System;
using Payroll.Common.Enums;

namespace Payroll.Service.Interfaces
{
    public interface IAttendanceService
    {
        int CreateWorkSchedule(string employeeCode, AttendanceCode attCode, DateTime datetime);
    }
}
