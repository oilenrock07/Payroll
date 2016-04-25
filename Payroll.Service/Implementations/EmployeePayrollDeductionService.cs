using Payroll.Entities;
using Payroll.Entities.Payroll;
using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Service.Implementations
{
    public class EmployeePayrollDeductionService : IEmployeePayrollDeductionService
    {
        private ISettingService _settingService;
        private IEmployeeSalaryService _employeeSalaryService;
        private IEmployeeInfoService _employeeInfoService;

        private readonly String IS_DEDUCTION_SEMIMONTHLY    = "DEDUCTION_IS_SEMIMONTHLY";

        private readonly String DEDUCTION_SEMIMONTHLY_SCHEDULE_1 = "DEDUCTION_SEMIMONTHLY_SCHEDULE_1";
        private readonly String DEDUCTION_SEMIMONTHLY_SCHEDULE_2 = "DEDUCTION_SEMIMONTHLY_SCHEDULE_2";
        private readonly String DEDUCTION_MONTHLY_SCHEDULE = "DEDUCTION_MONTHLY_SCHEDULE";

        private readonly String SEMIMONTHLY_TOTAL_HOURS     = "DEDUCTION_SEMIMONTHLY_TOTAL_HOURS";
        private readonly String MONTHLY_TOTAL_HOURS         = "DEDUCTION_MONTHLY_TOTAL_HOURS";

        public EmployeePayrollDeductionService(ISettingService settingService, IEmployeeSalaryService employeeSalaryService, IEmployeeInfoService employeeInfoService)
        {
            _settingService = settingService;
            _employeeSalaryService = employeeSalaryService;
            _employeeInfoService = employeeInfoService;
        }

        public void GenerateDeductionsByPayroll(DateTime payrollStartDate, DateTime payrollEndDate)
        {
            //Get settings if monthly or semimonthly
            bool isSemiMonthly = _settingService.GetByKey(IS_DEDUCTION_SEMIMONTHLY).Equals("1");
            bool proceed = false;

            //Check if payroll should have deduction
            if (isSemiMonthly)
            {
                int firstDeductionSchedule = Convert
                    .ToInt32(_settingService.GetByKey(DEDUCTION_SEMIMONTHLY_SCHEDULE_1));

                int secondDeductionSchedule = Convert
                   .ToInt32(_settingService.GetByKey(DEDUCTION_SEMIMONTHLY_SCHEDULE_2));

                if ((payrollStartDate.Day <= firstDeductionSchedule && 
                        payrollEndDate.Day >= firstDeductionSchedule) ||
                    (payrollStartDate.Day <= secondDeductionSchedule &&
                        payrollEndDate.Day >= secondDeductionSchedule))
                {
                    proceed = true;
                }
            }
            else
            {
                int deductionSchedule = Convert
                   .ToInt32(_settingService.GetByKey(DEDUCTION_MONTHLY_SCHEDULE));

                if (payrollStartDate.Day <= deductionSchedule &&
                        payrollEndDate.Day >= deductionSchedule)
                {
                    proceed = true;
                }
            }

            //If proceed is false return
            if (!proceed)
            {
                return;
            }

            double totalHours = 0;
            //Get total number of hours
            if (isSemiMonthly)
            {
                totalHours = Double.Parse(_settingService.GetByKey(SEMIMONTHLY_TOTAL_HOURS));
            }
            else
            {
                totalHours = Double.Parse(_settingService.GetByKey(MONTHLY_TOTAL_HOURS));
            }

            //Get employees
            var employeeList = _employeeInfoService.GetAllActive();

            foreach(EmployeeInfo employee in employeeList)
            {
                //Compute basic pay
                decimal basicPay = (_employeeSalaryService.GetEmployeeHourlyRate(employee) * (decimal)totalHours);

                //Compute HDMF Deduction


                //Compute SSS contribution
            }

            throw new NotImplementedException();
        }

    }
}
