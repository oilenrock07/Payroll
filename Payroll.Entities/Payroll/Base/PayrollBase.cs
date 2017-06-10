using System;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;

namespace Payroll.Entities.Payroll.Base
{
    public abstract class PayrollBase : BaseEntity
    {
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        public decimal TotalNet { get; set; }

        public decimal TotalGross { get; set; }

        public decimal TotalDeduction { get; set; }

        public decimal TaxableIncome { get; set; }

        public decimal TotalAdjustment { get; set; }

        public decimal TotalAllowance { get; set; }

        public DateTime PayrollDate { get; set; }

        public DateTime CutOffStartDate { get; set; }

        public DateTime CutOffEndDate { get; set; }

        public DateTime PayrollGeneratedDate { get; set; }

        public bool IsTaxed { get; set; }
    }
}
