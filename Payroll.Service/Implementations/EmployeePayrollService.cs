using Payroll.Common.Extension;
using Payroll.Entities;
using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
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
    public class EmployeePayrollService : IEmployeePayrollService
    {
        private IUnitOfWork _unitOfWork;
        private IEmployeePayrollRepository _employeePayrollRepository;
        private IEmployeeDailyPayrollService _employeeDailyPayrollService;
        private IEmployeePayrollDeductionService _employeePayrollDeductionService;
        private ISettingService _settingService;
        private IEmployeeInfoService _employeeInfoService;

        private readonly String PAYROLL_FREQUENCY = "PAYROLL_FREQUENCY";
        private readonly String PAYROLL_WEEK_START = "PAYROLL_WEEK_START";
        private readonly String PAYROLL_WEEK_END = "PAYROLL_WEEK_END";
        private readonly String ALLOWANCE_WEEK_SCHEDULE = "ALLOWANCE_WEEK_SCHEDULE";
        private readonly String ALLOWANCE_DAY_SCHEDULE = "ALLOWANCE_DAY_SCHEDULE";
        private readonly String ALLOWANCE_TOTAL_DAYS = "ALLOWANCE_TOTAL_DAYS";

        public EmployeePayrollService(IUnitOfWork unitOfWork, IEmployeeDailyPayrollService employeeDailyPayrollService, 
            IEmployeePayrollRepository employeeePayrollRepository, ISettingService settingService, IEmployeePayrollDeductionService employeePayrollDeductionService,
            IEmployeeInfoService employeeInfoService)
        {
            _unitOfWork = unitOfWork;
            _employeeDailyPayrollService = employeeDailyPayrollService;
            _employeePayrollRepository = employeeePayrollRepository;
            _settingService = settingService;
            _employeePayrollDeductionService = employeePayrollDeductionService;
            _employeeInfoService = employeeInfoService;
        }

        public IList<EmployeePayroll> GeneratePayrollGrossPayByDateRange(DateTime payrollDate, DateTime dateFrom, DateTime dateTo)
        {
            var employeeDailyPayroll = _employeeDailyPayrollService.GetByDateRange(dateFrom, dateTo);
            var employeePayrollList = new List<EmployeePayroll>();

            if (employeeDailyPayroll != null && employeeDailyPayroll.Count() > 0)
            {
                //Hold last payroll processed
                EmployeePayroll tempEmployeePayroll = null;
                DateTime today = new DateTime();

                EmployeeDailyPayroll last = employeeDailyPayroll.Last();

                foreach (EmployeeDailyPayroll dailyPayroll in employeeDailyPayroll)
                {
                    //If should create new entry
                    if (tempEmployeePayroll == null ||
                        (tempEmployeePayroll.EmployeeId != dailyPayroll.EmployeeId))
                    {
                        if (tempEmployeePayroll != null)
                        {
                            //Save last entry if for different employee
                            _employeePayrollRepository.Add(tempEmployeePayroll);
                            employeePayrollList.Add(tempEmployeePayroll);
                        }

                        EmployeePayroll employeePayroll = new EmployeePayroll
                        {
                            EmployeeId = dailyPayroll.EmployeeId,
                            CutOffStartDate = dateFrom,
                            CutOffEndDate = dateTo,
                            PayrollGeneratedDate = today,
                            PayrollDate = payrollDate,
                            TotalGross = dailyPayroll.TotalPay,
                            TotalNet = dailyPayroll.TotalPay,
                            TaxableIncome = dailyPayroll.TotalPay
                        };

                        tempEmployeePayroll = employeePayroll;

                    }
                    else
                    {
                        //Update last entry
                        tempEmployeePayroll.TotalGross += dailyPayroll.TotalPay;
                        tempEmployeePayroll.TotalNet += dailyPayroll.TotalPay;
                        tempEmployeePayroll.TaxableIncome += dailyPayroll.TotalPay;
                    }

                    //If last iteration save
                    if (dailyPayroll.Equals(last))
                    {
                        _employeePayrollRepository.Add(tempEmployeePayroll);
                        employeePayrollList.Add(tempEmployeePayroll);
                    }
                }

                //Commit
                _unitOfWork.Commit();
            }
            return employeePayrollList;
        }

        public void Update(EmployeePayroll employeePayroll)
        {
            _employeePayrollRepository.Update(employeePayroll);
        }

        public IList<EmployeePayroll> GetForTaxProcessingByEmployee(int employeeId, DateTime payrollDate)
        {
            return _employeePayrollRepository.GetForTaxProcessingByEmployee(employeeId, payrollDate);
        }

        public DateTime GetNextPayrollStartDate(FrequencyType frequency, DateTime? date)
        {
            DateTime? payrollStartDate = _employeePayrollRepository.GetNextPayrollStartDate();
            if (payrollStartDate == null)
            {
                var d = DateTime.Now;
                if (date != null)
                {
                    d = date.Value;
                }
                //TODO more frequency support
                switch (frequency)
                {
                    case FrequencyType.Weekly:
                        //Note that the job should always schedule the day after the payroll end date
                        var startOfWeeklyPayroll = (DayOfWeek)Enum.Parse(typeof(DayOfWeek),
                            _settingService.GetByKey(PAYROLL_WEEK_START));
                        
                        if (d.DayOfWeek == startOfWeeklyPayroll)
                        {
                            d = d.AddDays(-7);
                        }

                        payrollStartDate = d.StartOfWeek(startOfWeeklyPayroll);

                        break;
                }
            }
            return payrollStartDate.Value;
        }

        public DateTime GetNextPayrollEndDate(FrequencyType frequency, DateTime payrollStartDate)
        {
            DateTime payrollEndDate = payrollStartDate;

            //TODO more frequency support
            switch (frequency)
            {
                case FrequencyType.Weekly:
                    //Note that the job should always schedule the day after the payroll end date
                    var endOfWeekPayroll = (DayOfWeek)Enum.Parse(typeof(DayOfWeek),
                        _settingService.GetByKey(PAYROLL_WEEK_END));

                    payrollEndDate = payrollStartDate.AddDays(7).StartOfWeek(endOfWeekPayroll);

                    break;
            }
        
            return payrollEndDate;
        }

        public void GeneratePayroll(DateTime? date)
        {
            var frequency = (FrequencyType)Convert
                .ToInt32(_settingService.GetByKey(PAYROLL_FREQUENCY));

            DateTime payrollStartDate = GetNextPayrollStartDate(frequency, date);
            DateTime payrollEndDate = GetNextPayrollEndDate(frequency, payrollStartDate);
            
            GeneratePayroll(DateTime.Now, payrollStartDate, payrollEndDate);
        }

        public void GeneratePayroll(DateTime payrollDate, DateTime payrollStartDate, DateTime payrollEndDate)
        {
            //Generate employee payroll and net pay
            var employeePayrollList = GeneratePayrollGrossPayByDateRange(payrollDate, payrollStartDate, payrollEndDate);

            //Generate deductions such as SSS, HDMF, Philhealth and TAX
            _employeePayrollDeductionService.GenerateDeductionsByPayroll(payrollDate,
                payrollStartDate, payrollEndDate, employeePayrollList);

            //Generate Allowance

        }

        public IList<EmployeePayroll> GetByDateRange(DateTime dateStart, DateTime dateEnd)
        {
            dateEnd = dateEnd.AddDays(1);
            return _employeePayrollRepository.GetByDateRange(dateStart, dateEnd);
        }

        public void GenerateAllowance(DateTime payrollStartDate, DateTime payrollEndDate)
        {
            //Get allowance schedule
            //If 1st, 2nd, 3rd or 4th of the week
            int weekSchedule = Convert.ToInt32(_settingService.GetByKey(ALLOWANCE_WEEK_SCHEDULE));
            //If monday, tuesday ... so on
            DayOfWeek daySchedule = (DayOfWeek)Enum.Parse(typeof(DayOfWeek),
                            _settingService.GetByKey(ALLOWANCE_DAY_SCHEDULE));
            
            //Get schedule
            var allowanceDateByStartDate = DatetimeExtension
                .GetNthWeekofMonth(payrollStartDate, weekSchedule, daySchedule);

            var allowanceDateByEndDate = DatetimeExtension
               .GetNthWeekofMonth(payrollEndDate, weekSchedule, daySchedule);

            if ((allowanceDateByStartDate >= payrollStartDate && allowanceDateByStartDate < payrollStartDate.AddDays(1)) ||
                    (allowanceDateByEndDate >= payrollStartDate && allowanceDateByEndDate < payrollStartDate.AddDays(1)))
            {
                //Generate allowance
                var totalDays = Convert.ToInt32(_settingService.GetByKey(ALLOWANCE_TOTAL_DAYS));
                // Get all active employees
                var employees = _employeeInfoService.GetAllWithAllowance();

                foreach (EmployeeInfo employee in employees)
                {
                    //Compute here
                }
            }
        }
    }
}
