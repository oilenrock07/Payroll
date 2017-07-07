using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Interfaces;
using Payroll.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Service.Interfaces
{
    public interface IEmployeePayrollPerCompanyService
    {
        IList<EmployeePayrollPerCompany> GeneratePayrollGrossPayByDateRange(DateTime payrollDate, DateTime dateFrom, DateTime dateTo);

        void Update(EmployeePayrollPerCompany employeePayroll);

        void GeneratePayroll(DateTime? date);

        void GeneratePayroll(DateTime dateFrom, DateTime dateTo);

        /*
         * This will return payroll with payroll date between date start and date end
         */
        IList<EmployeePayrollPerCompany> GetByDateRange(DateTime dateStart, DateTime dateEnd);

        EmployeePayrollPerCompany GetById(int id);

        bool IsPayrollComputed(DateTime startDate, DateTime endDate);
    }
}
