using Payroll.Entities.Base;
using Payroll.Entities.Payroll;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("employee_payroll_deduction")]
    public class EmployeePayrollDeduction : BaseEntity
    {
        [Key]
        public int EmployeePayrollDeductionId { get; set; }

        public int EmployeeId { get; set; }

        [ForeignKey("Deduction")]
        public int DeductionId { get; set; }
        public virtual Deduction Deduction { get; set; }

        public int EmployeePayrollId { get; set; }

        public DateTime DeductionDate { get; set; }

        public decimal Amount { get; set; }

    }
}
