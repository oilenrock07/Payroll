using Payroll.Entities;
using Payroll.Entities.Payroll;
using System.Collections.Generic;
using System.Linq;

namespace Payroll.Models.Payroll
{
    public class PayrollDetailsViewModel
    {
        public EmployeePayroll Payroll { get; set; }
        public IEnumerable<EmployeePayrollItem> PayrollItems { get; set; }
        public IEnumerable<EmployeePayrollDeduction> Deductions { get; set; }
        public IEnumerable<EmployeeAdjustment> Adjustments { get; set; }


        public decimal TotalPayrollItems
        {
            get
            {
                return PayrollItems != null ? PayrollItems.Sum(x => x.TotalAmount) : 0;
            }
        }
    }
}