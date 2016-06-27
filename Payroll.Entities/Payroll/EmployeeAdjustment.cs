using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;

namespace Payroll.Entities.Payroll
{
    [Table("employee_adjustment")]
    public class EmployeeAdjustment : BaseEntity
    {
        [Key]
        public int EmployeeAdjustmentId { get; set; }

        [ForeignKey("Adjustment")]
        public int AdjustmentId { get; set; }
        public virtual Adjustment Adjustment { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        public DateTime Date { get; set; }

        public decimal Amount { get; set; }

        [StringLength(5000)]
        public string Remarks { get; set; }
    }
}
