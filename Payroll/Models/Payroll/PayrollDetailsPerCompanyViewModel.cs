using Payroll.Entities;
using Payroll.Entities.Payroll;
using System.Collections.Generic;
using System.Linq;

namespace Payroll.Models.Payroll
{
    public class PayrollDetailsPerCompanyViewModel
    {
        public EmployeePayrollPerCompany Payroll { get; set; }
        public IEnumerable<EmployeePayrollItemPerCompany> PayrollItems { get; set; }

        public decimal TotalPayrollItems
        {
            get
            {
                return PayrollItems != null ? PayrollItems.Sum(x => x.TotalAmount) : 0;
            }
        }
    }
}