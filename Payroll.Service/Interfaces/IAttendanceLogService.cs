using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Service.Interfaces
{
    public interface IAttendanceLogService : IBaseEntityService<AttendanceLog>
    {
        IList<AttendanceLog> GetAttendanceLogsToBeProcessed(DateTime fromDate, DateTime toDate);

        IList<AttendanceLog> GetAttendanceLogsToBeProcessed();
    }
}
