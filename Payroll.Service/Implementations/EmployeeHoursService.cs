using Payroll.Common.Extension;
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
        private readonly ISettingService _settingService;

        public EmployeeHoursService(IEmployeeHoursRepository employeeHoursRepository,
          IUnitOfWork unitOfWork, IAttendanceService attendanceService, ISettingService settingService)
        {
            _employeeHoursRepository = employeeHoursRepository;
            _unitOfWork = unitOfWork;
            _attendanceService = attendanceService;
            _settingService = settingService;

        }

        public int GenerateEmployeeHours(int PaymentFrequencyId, DateTime fromDate, DateTime toDate)
        {
            //Get all active employee with the same frequency
            IList<Employee> employees = _employeeService.GetActiveByPaymentFrequency(PaymentFrequencyId);

            var advanceOTEndTimeSettings =
                           _settingService.GetByKey("SCHEDULE_ADVANCE_OT_END");

            foreach (var employee in employees)
            {
                foreach (DateTime day in DatetimeExtension.EachDay(fromDate, toDate))
                {
                    //Get all employee attendance within date range
                    IList<Attendance> attendanceList = _attendanceService
                        .GetAttendanceByDate(employee.EmployeeId, day);

                    //var advanceOTEndTime = ;

                    //Compute hours
                    foreach (var attendance in attendanceList)
                    {
                        // Early OT or OT of from yesterday
                        //  This may be special for client
                       // if (attendance.ClockIn < )
                        //{

                        //}
                        // * Regular Hours
                        // Consider 


                        // * OT hours

                        // * Night Dif

                    }
                }
            }
            return 0;
        }
    }
}
