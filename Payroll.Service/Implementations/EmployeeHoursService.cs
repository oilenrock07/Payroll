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
        private readonly IEmployeeWorkScheduleService _employeeWorkScheduleService;

        public EmployeeHoursService(IEmployeeHoursRepository employeeHoursRepository,
          IUnitOfWork unitOfWork, IAttendanceService attendanceService, ISettingService settingService,
          IEmployeeWorkScheduleService employeeWorkScheduleService)
        {
            _employeeHoursRepository = employeeHoursRepository;
            _unitOfWork = unitOfWork;
            _attendanceService = attendanceService;
            _settingService = settingService;
            _employeeWorkScheduleService = employeeWorkScheduleService;
        }

        public int GenerateEmployeeHours(int PaymentFrequencyId, DateTime fromDate, DateTime toDate)
        {
            //Get all active employee with the same frequency
            IList<Employee> employees = _employeeService.GetActiveByPaymentFrequency(PaymentFrequencyId);

            var advanceOTPeriod =
                          Int32.Parse(_settingService.GetByKey("SCHEDULE_ADVANCE_OT_PERIOD_MINUTES"));

            foreach (var employee in employees)
            {
                var employeeWorkSchedule = _employeeWorkScheduleService.GetByEmployeeId(employee.EmployeeId);

                foreach (DateTime day in DatetimeExtension.EachDay(fromDate, toDate))
                {
                    //Get all employee attendance within date range
                    IList<Attendance> attendanceList = _attendanceService
                        .GetAttendanceByDate(employee.EmployeeId, day);

                    //Compute hours
                    foreach (var attendance in attendanceList)
                    {
                        // Early OT or OT of from yesterday
                        //  This may be special for client
                        // Check if within advance OT 
                       TimeSpan scheduledTimeIn = employeeWorkSchedule.WorkSchedule.TimeStart;
                       TimeSpan actualTimeIn = attendance.ClockIn.TimeOfDay;

                       /*if (actualTimeIn < scheduledTimeIn &&
                            DateTime.Parse(scheduledTimeIn).Subtract(DateTime.Parse(actualTimeIn).);
                        scheduledTimeIn.Subtract(actualTimeIn) > advanceOTPeriod)
                        {
                            // Count hour before 12 midnight

                        }else if (attendance.ClockIn == advanceOTEndTime &&
                                        attendance.ClockIn < employeeInfo.)
                        {
                            // Else if within 

                        }*/
                        // * Regular Hours
                        // Consider 


                        // * OT hours

                        // * Night Dif

                        // Rest Day

                        // Holiday

                    }
                }
            }
            return 0;
        }
    }
}
