using Payroll.Entities;
using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Service.Implementations
{
    public class EmployeePayrollAllowanceService : IEmployeePayrollAllowanceService
    {
        private ISettingService _settingService;
        private ITotalEmployeeHoursService _totalEmployeeHourService;

        private readonly String IS_ALLOWANCE_SEMIMONTHLY = "ALLOWANCE_IS_SEMIMONTHLY";

        private readonly String ALLOWANCE_SEMIMONTHLY_SCHEDULE_1 = "ALLOWANCE_SEMIMONTHLY_SCHEDULE_1";
        private readonly String ALLOWANCE_SEMIMONTHLY_SCHEDULE_2 = "ALLOWANCE_SEMIMONTHLY_SCHEDULE_2";
        private readonly String ALLOWANCE_MONTHLY_SCHEDULE = "ALLOWANCE_MONTHLY_SCHEDULE";

        public EmployeePayrollAllowanceService(ISettingService settingService, ITotalEmployeeHoursService totalEmployeeHourService)
        {
            _settingService = settingService;
            _totalEmployeeHourService = totalEmployeeHourService;
        }
        public decimal ComputeEmployeeAllowance(int totalDays, int totalWorkHoursPerDay,
            EmployeeInfo employee, DateTime payrollStartDate, DateTime payrollEndDate)
        {
            double totalHours = 0;
            //Get regular and OT hours per day
            List<TotalEmployeeHours> employeeTotalHours =
               new List<TotalEmployeeHours>(_totalEmployeeHourService
                   .GetByTypeAndDateRange(employee.EmployeeId, RateType.Regular, payrollStartDate, payrollEndDate));

            employeeTotalHours.AddRange(_totalEmployeeHourService
                   .GetByTypeAndDateRange(employee.EmployeeId, RateType.OverTime, payrollStartDate, payrollEndDate));

            employeeTotalHours = employeeTotalHours.OrderByDescending(e => e.Date).ToList();

            if (employeeTotalHours != null && employeeTotalHours.Count > 1)
            {
                DateTime? tempDate = null;
                double dayHours = 0;

                var last = employeeTotalHours.Last();
                foreach (TotalEmployeeHours employeeHours in employeeTotalHours)
                {
                    //If different date add dayhours to totalhours and set dat hours to 0
                    if (tempDate != null && tempDate != employeeHours.Date)
                    {
                        totalHours += (dayHours > totalWorkHoursPerDay ?
                            totalWorkHoursPerDay : dayHours);
                        dayHours = 0;
                    }

                    dayHours = dayHours + employeeHours.Hours;
                    tempDate = employeeHours.Date;

                    //If last iteration
                    if (last.Equals(employeeHours))
                    {
                        totalHours += (dayHours > totalWorkHoursPerDay
                            ? totalWorkHoursPerDay : dayHours);
                    }
                }

                //Compute total allowance
               
                var totalAllowanceHours = totalDays * totalWorkHoursPerDay;

                Decimal totalAllowancePerHour = employee.Allowance.Value /
                        ((decimal)totalDays * (decimal)totalWorkHoursPerDay);

                return (decimal)totalHours * totalAllowancePerHour;
            }
            else
            {
                return 0;
            }
        }

        public bool proceedAllowance(DateTime payrollStartDate, DateTime payrollEndDate)
        {
            //Get settings if monthly or semimonthly
            bool isSemiMonthly = _settingService.GetByKey(IS_ALLOWANCE_SEMIMONTHLY).Equals("1");
            bool proceed = false;

            //Check if payroll should have deduction
            if (isSemiMonthly)
            {
                int firstAllowanceSchedule = Convert
                    .ToInt32(_settingService.GetByKey(ALLOWANCE_SEMIMONTHLY_SCHEDULE_1));

                int secondAllowanceSchedule = Convert
                   .ToInt32(_settingService.GetByKey(ALLOWANCE_SEMIMONTHLY_SCHEDULE_2));

                if ((payrollStartDate.Day <= firstAllowanceSchedule &&
                        payrollEndDate.Day >= firstAllowanceSchedule) ||
                    (payrollStartDate.Day <= secondAllowanceSchedule &&
                        payrollEndDate.Day >= secondAllowanceSchedule))
                {
                    proceed = true;
                }
            }
            else
            {
                int allowanceSchedule = Convert
                   .ToInt32(_settingService.GetByKey(ALLOWANCE_MONTHLY_SCHEDULE));

                //This will handle end of month
                int lastDayOfMonth1 = DateTime.DaysInMonth(payrollStartDate.Year, payrollStartDate.Month);
                int allowanceSchedule1 = allowanceSchedule <= lastDayOfMonth1 ? allowanceSchedule : lastDayOfMonth1;

                //Use month of start date
                DateTime allowanceDate =
                    new DateTime(payrollStartDate.Year, payrollStartDate.Month, allowanceSchedule1);

                //This will handle end of month
                int lastDayOfMonth2 = DateTime.DaysInMonth(payrollEndDate.Year, payrollEndDate.Month);
                int allowanceSchedule2 = allowanceSchedule <= lastDayOfMonth2 ? allowanceSchedule : lastDayOfMonth2;

                //Use month of end date since it can be diff month
                DateTime allowanceDate2 =
                    new DateTime(payrollEndDate.Year, payrollEndDate.Month, allowanceSchedule2);

                if ((payrollStartDate <= allowanceDate && payrollEndDate >= allowanceDate)
                        || (payrollStartDate <= allowanceDate2 && payrollEndDate >= allowanceDate2))
                {
                    proceed = true;
                }
            }

            return proceed;
        }
    }
}
