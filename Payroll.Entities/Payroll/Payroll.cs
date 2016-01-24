using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities.Payroll
{
    [Table("tbl_payroll")]
    public class Payroll
    {
        [Key]
        public int PayrollId { get; set; }

        public int EmployeeId { get; set; }

        public decimal Salary { get; set; }

        public decimal TotalDeduction { get; set; }

        public decimal TotalAdjustment { get; set; }

        public DateTime PayrollDate { get; set; }

        public DateTime CutOffStartDate { get; set; }

        public DateTime CutOffEndDate { get; set; }

        public DateTime PayrollGeneratedDate { get; set; }

        public decimal TotalPay { get; set; }
    }
}
