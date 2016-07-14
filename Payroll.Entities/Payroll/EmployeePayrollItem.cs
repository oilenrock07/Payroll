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

        public int? PayrollId { get; set; }

        public DateTime PayrollDate { get; set; }

        public RateType RateType { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        public double TotalHours { get; set; }

        public double Multiplier { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal RatePerHour { get; set; }

    }
}
