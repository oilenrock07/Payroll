using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            EmployeeSalary employeeSalary = employeeInfo.EmployeeSalary;

            Decimal hourlyRate = employeeSalary.Salary;

            //TODO more salary frequency
            switch (employeeSalary.SalaryFrequency)
            {
                case SalaryFrequency.Hourly:
                    {
                        hourlyRate = (employeeSalary.Salary / SALARY_HOURLY);
                        break;
                    }
                case SalaryFrequency.Daily:
                    {
                        hourlyRate = (employeeSalary.Salary / SALARY_DAILY);
                        break;
                    }
                case SalaryFrequency.Weekly:
                    {
                        hourlyRate = (employeeSalary.Salary / SALARY_WEEKLY);
                        break;
                    }
                case SalaryFrequency.BiWeekly:
                    {
                        hourlyRate = (employeeSalary.Salary / SALARY_BIWEEKLY);
                        break;
                    }
            }

            return hourlyRate;
        }
    }
}
