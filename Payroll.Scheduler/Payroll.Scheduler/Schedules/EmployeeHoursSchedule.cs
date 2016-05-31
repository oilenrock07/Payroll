using Payroll.Common.Extension;
using Payroll.Entities.Contexts;
using Payroll.Entities.Enums;
using Payroll.Infrastructure.Implementations;
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
        private readonly PayrollContext _payrollContext;
        private readonly IDatabaseFactory _databaseFactory;
        private readonly IUnitOfWork _unitOfWork;

        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IAttendanceLogRepository _attendanceLogRepository;
        private readonly ISettingRepository _settingRepository;
        private readonly IEmployeeWorkScheduleRepository _employeeWorkScheduleRepository;
        private readonly IEmployeeHoursRepository _employeeHoursRepository;
        private readonly IEmployeeInfoRepository _employeeInfoRepository;
        private readonly IEmployeePayrollRepository _employeePayrollRepository;

        private readonly IEmployeeInfoService _employeeInfoService;
        private readonly IAttendanceLogService _attendanceLogService;
        private readonly IAttendanceService _attendanceService;
        private readonly ISettingService _settingService;
        private readonly IEmployeeWorkScheduleService _employeeWorkScheduleService;
        private readonly IEmployeeHoursService _employeeHoursService;
        private readonly IEmployeePayrollService _employeePayrollService;

        private const string PAYROLL_FREQUENCY = "PAYROLL_FREQUENCY";

       public EmployeeHoursSchedule()
        {
            _payrollContext = new PayrollContext();
            _databaseFactory = new DatabaseFactory(_payrollContext);
            _unitOfWork = new UnitOfWork(_databaseFactory);

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
            var frequency = (FrequencyType)Int32
                .Parse(_settingService.GetByKey(PAYROLL_FREQUENCY));
            var payrollStartDate = _employeePayrollService
                .GetNextPayrollStartDate(frequency, DateTime.Now).TruncateTime();
            var payrollEndDate = _employeePayrollService
                .GetNextPayrollEndDate(frequency, payrollStartDate).TruncateTime();

            //Compute employee hours
            Console.WriteLine("Computing daily employee hours for date " + payrollStartDate + " to " + payrollEndDate);
            _employeeHoursService.GenerateEmployeeHours(payrollStartDate, payrollEndDate);
        }
    }
}
