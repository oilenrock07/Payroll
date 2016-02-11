using Payroll.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Service.Interfaces
{
    public interface IAttendanceLogService
    {
        IList<AttendanceLog> GetAttendanceLogsToBeProcessed(string employeeCode, DateTime fromDate, DateTime toDate);

        void Save(AttendanceLog attendanceLog);
    }
}
