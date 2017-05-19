using Payroll.Entities.Base;
using Payroll.Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities.Payroll
{
    [Table("employee_hours_total")]
    public class TotalEmployeeHours : BaseEntity
    {
        [Key]
        public int TotalEmployeeHoursId { get; set; }
        
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        public DateTime Date { get; set; }

        public double Hours { get; set; }

        public RateType Type { get; set; }
    }
}
