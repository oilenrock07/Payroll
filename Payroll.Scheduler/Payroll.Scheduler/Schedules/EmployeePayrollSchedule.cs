using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payroll.Entities.Payroll;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
using Payroll.Scheduler.Interfaces;
using Payroll.Service.Interfaces;
using Payroll.Entities.Contexts;
using Payroll.Infrastructure.Interfaces;
using Payroll.Infrastructure.Implementations;
using Payroll.Service.Implementations;
using Payroll.Service;
using Payroll.Entities.Enums;
using Payroll.Common.Extension;

namespace Payroll.Scheduler.Schedules
{
    public class EmployeePayrollSchedule : ISchedule
    {
        public readonly PayrollContext _payrollContext;
        public readonly IDatabaseFactory _databaseFactory;
        public readonly IUnitOfWork _unitOfWork;

        private readonly IEmployeeHoursRepository _employeeHoursRepository;
        private readonly ITotalEmployeeHoursRepository _totalEmployeeHoursRepository;
        private readonly IEmployeeDepartmentRepository _employeeDepartmentRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IAttendanceLogRepository _attendanceLogRepository;
        private readonly ISettingRepository _settingRepository;
        private readonly IEmployeeWorkScheduleRepository _employeeWorkScheduleRepository;
        private readonly IEmployeeInfoRepository _employeeInfoRepository;
        private readonly IFrequencyRepository _frequencyRepository;
        private readonly IPaymentFrequencyRepository _paymentFrequencyRepository;
        private readonly IEmployeePayrollRepository _employeePayrollRepository;
        private readonly IEmployeePayrollDeductionRepository _employeePayrollDeductionRepository;
        private readonly IEmployeeDailyPayrollRepository _employeeDailyPayrollRepository;
        private readonly IHolidayRepository _holidayRepository;
        private readonly IEmployeeDeductionRepository _employeeDeductionRepository;
        private readonly IDeductionRepository _deductionRepository;
        private readonly ITaxRepository _taxRepository;

        private readonly IEmployeeInfoService _employeeInfoService;
        private readonly IAttendanceLogService _attendanceLogService;
        private readonly IAttendanceService _attendanceService;
        private readonly ISettingService _settingService;
        private readonly IEmployeeWorkScheduleService _employeeWorkScheduleService;
        private readonly IEmployeeHoursService _employeeHoursService;
        private readonly ITotalEmployeeHoursService _totalEmployeeHoursService;
        private readonly IEmployeeDailyPayrollService _employeeDailyPayrollService;
        private readonly IEmployeePayrollDeductionService _employeePayrollDeductionService;
        private readonly IEmployeePayrollService _employeePayrollService;
        private readonly IHolidayService _holidayService;
        private readonly IEmployeeSalaryService _employeeSalaryService;
        private readonly IEmployeeDeductionService _employeeDeductionService;
        private readonly IDeductionService _deductionService;
        private readonly ITaxService _taxService;

        private const string PAYROLL_FREQUENCY = "PAYROLL_FREQUENCY";

        public EmployeePayrollSchedule()
        {
            _payrollContext = new PayrollContext();
            _databaseFactory = new DatabaseFactory(_payrollContext);
            _unitOfWork = new UnitOfWork(_databaseFactory);

            _employeeDepartmentRepository = new EmployeeDepartmentRepository(_databaseFactory);
            _employeeRepository = new EmployeeRepository(_databaseFactory, _employeeDepartmentRepository);
            _attendanceRepository = new AttendanceRepository(_databaseFactory);
            _attendanceLogRepository = new AttendanceLogRepository(_databaseFactory, _employeeRepository);
            _settingRepository = new SettingRepository(_databaseFactory);
            _employeeWorkScheduleRepository = new EmployeeWorkScheduleRepository(_databaseFactory);
            _employeeInfoRepository = new EmployeeInfoRepository(_databaseFactory);
            _frequencyRepository = new FrequencyRepository(_databaseFactory);
            _paymentFrequencyRepository = new PaymentFrequencyRepository(_databaseFactory);
            _employeePayrollRepository = new EmployeePayrollRepository(_databaseFactory);
            _employeePayrollDeductionRepository = new EmployeePayrollDeductionRepository(_databaseFactory);
            _employeeDailyPayrollRepository = new EmployeeDailyPayrollRepository(_databaseFactory);
            _employeeHoursRepository = new EmployeeHoursRepository(_databaseFactory);
            _totalEmployeeHoursRepository = new TotalEmployeeHoursRepository(_databaseFactory);
            _holidayRepository = new HolidayRepository(_databaseFactory);
            _employeeDeductionRepository = new EmployeeDeductionRepository(_databaseFactory);
            _deductionRepository = new DeductionRepository(_databaseFactory);

            _employeeInfoService = new EmployeeInfoService(_employeeInfoRepository);
            _attendanceLogService = new AttendanceLogService(_attendanceLogRepository);
            _attendanceService = new AttendanceService(_unitOfWork, _attendanceRepository, _attendanceLogService);
            _settingService = new SettingService(_settingRepository);
            _employeeWorkScheduleService = new EmployeeWorkScheduleService(_employeeWorkScheduleRepository);
            _employeeSalaryService = new EmployeeSalaryService();
            _employeeHoursService = new EmployeeHoursService(_unitOfWork, _employeeHoursRepository,
                _attendanceService, _settingService, _employeeWorkScheduleService, _employeeInfoService);
            _totalEmployeeHoursService = new TotalEmployeeHoursService(_unitOfWork, _totalEmployeeHoursRepository, _employeeHoursService, _settingService);
            _holidayService = new HolidayService(_holidayRepository, _settingRepository, _unitOfWork);
            _employeeDailyPayrollService = new EmployeeDailyPayrollService(_unitOfWork, _totalEmployeeHoursService, _employeeWorkScheduleService, _holidayService,
                _settingService, _employeeDailyPayrollRepository, _employeeInfoService, _employeeSalaryService);
            _deductionService = new DeductionService(_deductionRepository);
            _taxService = new TaxService(_taxRepository);
            _employeePayrollDeductionService = new EmployeePayrollDeductionService(_unitOfWork, _settingService, _employeeSalaryService, _employeeInfoService, _employeeDeductionService, _deductionService, _employeePayrollDeductionRepository, _employeePayrollService, _taxService);
            _employeePayrollService = new EmployeePayrollService(_unitOfWork, _employeeDailyPayrollService, _employeePayrollRepository, _settingService, _employeePayrollDeductionService, _employeeInfoService, _totalEmployeeHoursService );
           
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

            //Compute total employee hours
            Console.WriteLine("Computing total employee hours for date " + payrollStartDate + " to " + payrollEndDate);
            _totalEmployeeHoursService.GenerateTotalByDateRange(payrollStartDate, payrollEndDate);

            //Compute daily payroll
            Console.WriteLine("Computing daily payroll for date " + payrollStartDate + " to " + payrollEndDate);
            _employeeDailyPayrollService.GenerateEmployeeDailySalaryByDateRange(payrollStartDate, payrollEndDate);

            //Compute total payroll
            Console.WriteLine("Computing total payroll for date " + payrollStartDate + " to " + payrollEndDate);
            _employeePayrollService.GeneratePayroll(DateTime.Now, payrollStartDate, payrollEndDate);
        }
    }
}
