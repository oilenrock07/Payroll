using System;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;
using Payroll.Entities.Enums;

namespace Payroll.Entities.Payroll.Base
{
    public class PayrollItemBase : BaseEntity
    {
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
