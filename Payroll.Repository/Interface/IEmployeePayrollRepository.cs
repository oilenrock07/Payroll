﻿using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Repository.Interface
{
    public interface IEmployeePayrollRepository : IRepository<EmployeePayroll>
    {
        IList<EmployeePayroll> GetForTaxProcessingByEmployee(int employeeId, DateTime payrollDate);

        /*
         * This will return payroll with payroll date between date start and date end
        */
        IList<EmployeePayroll> GetByDateRange(DateTime dateStart, DateTime dateEnd);

        //Get last payroll end date
        DateTime? GetNextPayrollStartDate();

        EmployeePayroll GetEmployeePayrollByDate(int employeeId, DateTime payrollStart, DateTime payrollEnd);
    }
}
