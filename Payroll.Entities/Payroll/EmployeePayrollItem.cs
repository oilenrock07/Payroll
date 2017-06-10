using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Payroll.Base;

namespace Payroll.Entities.Payroll
{
    [Table("employee_payroll_item")]
    public class EmployeePayrollItem : PayrollItemBase
    {
        [Key]
        public int EmployeePayrollItemId { get; set; }

        public int? PayrollId { get; set; }
    }
}
