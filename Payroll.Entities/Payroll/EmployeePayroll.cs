using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Payroll.Base;

namespace Payroll.Entities.Payroll
{
    [Table("payroll")]
    public class EmployeePayroll : PayrollBase
    {
        [Key]
        public int PayrollId { get; set; }         
    }
}
