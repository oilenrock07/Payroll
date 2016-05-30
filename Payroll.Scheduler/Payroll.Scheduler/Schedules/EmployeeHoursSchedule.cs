using Payroll.Entities.Contexts;
using Payroll.Entities.Enums;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
using Payroll.Scheduler.Interfaces;
using Payroll.Service.Implementations;
using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Scheduler.Schedules
{
    public class EmployeeHoursSchedule : ISchedule
    {
        public readonly PayrollContext _payrollContext;
        public readonly IDatabaseFactory _databaseFactory;
        public readonly IUnitOfWork _unitOfWork;

        public readonly IEmployeeRepository _employeeRepository;
        public readonly IAttendanceRepository _attendanceRepository;
        public readonly IAttendanceLogRepository _attendanceLogRepository;
        public readonly ISettingRepository _settingRepository;
        public readonly IEmployeeWorkScheduleRepository _employeeWorkScheduleRepository;
        public readonly IEmployeeHoursRepository _employeeHoursRepository;
        public readonly IEmployeeInfoRepository _employeeInfoRepository;
        public readonly IEmployeePayrollRepository _employeePayrollRepository;

        public readonly IEmployeeInfoService _employeeInfoService;
        public readonly IAttendanceLogService _attendanceLogService;
        public readonly IAttendanceService _attendanceService;
        public readonly ISettingService _settingService;
        public readonly IEmployeeWorkScheduleService _employeeWorkScheduleService;
        public readonly IEmployeeHoursService _employeeHoursService;
        public readonly IEmployeePayrollService _employeePayrollService;

        private readonly string PAYROLL_FREQUENCY = "PAYROLL_FREQUENCY";

       public EmployeeHoursSchedule()
        {
            _employeeRepository = new EmployeeRepository(_databaseFactory, null);
            _attendanceRepository = new AttendanceRepository(_databaseFactory);
            _attendanceLogRepository = new AttendanceLogRepository(_databaseFactory, _employeeRepository);
            _settingRepository = new SettingRepository(_databaseFactory);
            _employeeWorkScheduleRepository = new EmployeeWorkScheduleRepository(_databaseFactory);
            _employeeHoursRepository = new EmployeeHoursRepository(_databaseFactory);
            _employeeInfoRepository = new EmployeeInfoRepository(_databaseFactory);
            _employeePayrollRepository = new EmployeePayrollRepository(_databaseFactory);

            _employeeInfoService = new EmployeeInfoService(_employeeInfoRepository);
            _attendanceLogService = new AttendanceLogService(_attendanceLogRepository);
            _attendanceService = new AttendanceService(_unitOfWork, _attendanceRepository, _attendanceLogService);

            _settingService = new SettingService(_settingRepository);
            _employeeWorkScheduleService = new EmployeeWorkScheduleService(_employeeWorkScheduleRepository);

            _employeeHoursService = new EmployeeHoursService(_unitOfWork, _employeeHoursRepository, _attendanceService, _settingService, _employeeWorkScheduleService, _employeeInfoService);
            _employeePayrollService = new EmployeePayrollService(_unitOfWork, null, _employeePayrollRepository, _settingService, null, _employeeInfoService, null);

        }

        public void Execute()
        {
            //Get payroll date range
            FrequencyType frequency = (FrequencyType)Int32
                .Parse(_settingService.GetByKey(PAYROLL_FREQUENCY));
            var payrollStartDate = _employeePayrollService
                .GetNextPayrollStartDate(frequency, DateTime.Now);
            var payrollEndDate = _employeePayrollService
                .GetNextPayrollEndDate(frequency, payrollStartDate);

            //Compute employee hours
            _employeeHoursService.GenerateEmployeeHours(payrollStartDate, payrollEndDate);
        }
    }
}
