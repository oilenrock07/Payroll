using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;

namespace Payroll.Entities.Payroll
{
    [Table("payroll")]
    public class EmployeePayroll : BaseEntity
    {
        public EmployeePayroll(){
            TotalNet = 0;
            TotalDeduction = 0;
            TotalAdjustment = 0;
            TotalGross = 0;
            TaxableIncome = 0;
            TotalAllowance = 0;
            IsTaxed = false;
        }

        [Key]
        public int PayrollId { get; set; }

        public int EmployeeId { get; set; }

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
