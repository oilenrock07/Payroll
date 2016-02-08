using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Service.Implementations
{
    public class EmployeeHoursService : IEmployeeHoursService
    {
        private readonly IEmployeeHoursRepository _employeeHoursRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAttendanceService _attendanceService;
        private readonly IEmployeeService _employeeService;

        public EmployeeHoursService(IEmployeeHoursRepository employeeHoursRepository,
          IUnitOfWork unitOfWork, IAttendanceService attendanceService)
        {
            _employeeHoursRepository = employeeHoursRepository;
            _unitOfWork = unitOfWork;
            _attendanceService = attendanceService;
        }

        public int GenerateEmployeeHours(int PaymentFrequencyId, DateTime fromDate, DateTime toDate)
        {
            //Get all active employee with the same frequency
            IList<Employee> employees = _employeeService.GetActiveByPaymentFrequency(PaymentFrequencyId);

            foreach (var employee in employees)
            {
                //Get all employee attendance within date range
                IList<Attendance> attendanceList = _attendanceService
                    .GetAttendanceByDateRange(employee.EmployeeCode, fromDate, toDate);

                
            }
            return 0;
        }
    }
}
