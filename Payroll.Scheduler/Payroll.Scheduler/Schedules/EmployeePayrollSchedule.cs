using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payroll.Entities.Payroll;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
using Payroll.Scheduler.Interfaces;

namespace Payroll.Scheduler.Schedules
{
    public class EmployeePayrollSchedule : ISchedule
    {
        private readonly IEmployeeHoursRepository _employeeHoursRepository;
        private readonly ITotalEmployeeHoursRepository _totalEmployeeHoursRepository;
        private readonly IEmployeeDepartmentRepository _employeeDepartmentRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IAttendanceLogRepository _attendanceLogRepository;

        public EmployeePayrollSchedule()
        {
        }
        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
