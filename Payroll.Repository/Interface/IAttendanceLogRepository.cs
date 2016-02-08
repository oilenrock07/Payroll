using Payroll.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payroll.Entities;

namespace Payroll.Repository.Interface
{
    public interface IAttendanceLogRepository : IRepository<AttendanceLog>
    {
        IList<AttendanceLog> GetAttendanceLogs(DateTime fromDate, DateTime toDate, Boolean isRecorded);
    }
}
