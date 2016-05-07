using Payroll.Service.Interfaces;
using System;
using Payroll.Entities;
using Payroll.Entities.Enums;

namespace Payroll.Service
{
    public class EmployeeSalaryService : IEmployeeSalaryService
    {
        private readonly int SALARY_HOURLY = 1;
        private readonly int SALARY_DAILY = 8;
        private readonly int SALARY_WEEKLY = 40;
        private readonly int SALARY_BIWEEKLY = 80;

        public decimal GetEmployeeHourlyRate(EmployeeInfo employeeInfo)
        {
            //EmployeeSalary employeeSalary = employeeInfo.EmployeeSalary;

            Decimal hourlyRate = employeeInfo.Salary;

            //TODO more salary frequency
            switch (employeeInfo.SalaryFrequency)
            {
                case FrequencyType.Hourly:
                    {
                        hourlyRate = (employeeInfo.Salary / SALARY_HOURLY);
                        break;
                    }
                case FrequencyType.Daily:
                    {
                        hourlyRate = (employeeInfo.Salary / SALARY_DAILY);
                        break;
                    }
                case FrequencyType.Weekly:
                    {
                        hourlyRate = (employeeInfo.Salary / SALARY_WEEKLY);
                        break;
                    }
                case FrequencyType.BiWeekly:
                    {
                        hourlyRate = (employeeInfo.Salary / SALARY_BIWEEKLY);
                        break;
                    }
            }

            return hourlyRate;
        }
    }
}
