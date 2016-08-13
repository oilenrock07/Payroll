using Payroll.Common.Extension;
using Payroll.Entities.Enums;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
using Payroll.Scheduler.Interfaces;
using Payroll.Service.Implementations;
using Payroll.Service.Interfaces;
using System;

namespace Payroll.Scheduler.Schedules
{
    public class EmployeeHoursSchedule : BaseSchedule, ISchedule
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IAttendanceLogRepository _attendanceLogRepository;
        private readonly ISettingRepository _settingRepository;
        private readonly IEmployeeWorkScheduleRepository _employeeWorkScheduleRepository;
        private readonly IEmployeeHoursRepository _employeeHoursRepository;
        private readonly IEmployeeInfoRepository _employeeInfoRepository;

        private readonly IEmployeeInfoService _employeeInfoService;
        private readonly IAttendanceLogService _attendanceLogService;
        private readonly IAttendanceService _attendanceService;
        private readonly ISettingService _settingService;
        private readonly IEmployeeWorkScheduleService _employeeWorkScheduleService;
        private readonly IEmployeeHoursService _employeeHoursService;
        private readonly IEmployeePayrollService _employeePayrollService;
        private readonly ISchedulerLogRepository _schedulerLogRepository;
        private readonly ITotalEmployeeHoursService _totalEmployeeHoursService;
        private readonly IEmployeeService _employeeService;

        private const string PAYROLL_FREQUENCY = "PAYROLL_FREQUENCY";

       public EmployeeHoursSchedule()
        {
            _employeeRepository = new EmployeeRepository(_databaseFactory, null);
            _attendanceRepository = new AttendanceRepository(_databaseFactory);
            _attendanceLogRepository = new AttendanceLogRepository(_databaseFactory, _employeeRepository);
            _settingRepository = new SettingRepository(_databaseFactory);
            _employeeWorkScheduleRepository = new EmployeeWorkScheduleRepository(_databaseFactory);
            _employeeHoursRepository = new EmployeeHoursRepository(_databaseFactory);
            _employeeInfoRepository = new EmployeeInfoRepository(_databaseFactory);

            _employeeService = new EmployeeService(_employeeRepository);
            _employeeInfoService = new EmployeeInfoService(_employeeInfoRepository);
            _attendanceLogService = new AttendanceLogService(_attendanceLogRepository);
            _attendanceService = new AttendanceService(_unitOfWork, _attendanceRepository, _attendanceLogService, _employeeHoursRepository);
            _settingService = new SettingService(_settingRepository);
            _employeeWorkScheduleService = new EmployeeWorkScheduleService(_employeeWorkScheduleRepository);
            _employeeHoursService = new EmployeeHoursService(_unitOfWork, _employeeHoursRepository, _attendanceService, _settingService, _employeeWorkScheduleService, _employeeInfoService);

            _schedulerLogRepository = new SchedulerLogRepository(_databaseFactory);
        }

        public void Execute()
        {
            //Get payroll date range
            try
            {
                var payrollStartDate = _employeePayrollService
                    .GetNextPayrollStartDate(DateTime.Now).TruncateTime();
                var payrollEndDate = _employeePayrollService
                    .GetNextPayrollEndDate(payrollStartDate).TruncateTime();

                //Compute employee hours
                Console.WriteLine("Computing daily employee hours for date " + payrollStartDate + " to " +
                                  payrollEndDate);
                _employeeHoursService.GenerateEmployeeHours(payrollStartDate, payrollEndDate);
                LogSchedule(SchedulerLogType.Success);
            }
            catch (Exception ex)
            {
                LogSchedule(SchedulerLogType.Exception, ex.InnerException.Message);
            }

        }
    }
}
