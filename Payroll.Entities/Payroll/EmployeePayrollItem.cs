using Payroll.Entities.Base;
using Payroll.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Entities.Payroll
{
    [Table("employee_payroll_item")]
    public class EmployeePayrollItem : BaseEntity
    {
        [Key]
        public int EmployeePayrollItemId { get; set; }

        public RateType RateType { get; set; }

        [ForeignKey("EmployeePayroll")]
        public int PayrollId { get; set; }
        public virtual EmployeePayroll EmployeePayroll { get; set; }

        public double TotalHours { get; set; }

        public decimal TotalAmount { get; set; }
        
    }
}
