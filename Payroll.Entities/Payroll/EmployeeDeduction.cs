using Payroll.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities.Payroll
{
    [Table("employee_deduction")]
    public class EmployeeDeduction : BaseEntity
    {
        [Key]
        public int EmployeeDeductionId { get; set; }

        [ForeignKey("Deduction")]
        public int DeductionId { get; set; }
        public virtual Deduction Deduction { get; set; }

        public decimal Amount { get; set; }

        public int EmployeeId { get; set; }
    }
}
