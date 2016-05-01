using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;

namespace Payroll.Entities
{
    [Table("employee_workschedule")]
    public class EmployeeWorkSchedule : BaseEntity
    {
        [Key]
        public int EmployeeWorkScheduleId { get; set; }

        //[ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        //public virtual Employee Employee { get; set; }

        [ForeignKey("WorkSchedule")]
        public int WorkScheduleId { get; set; }
        public virtual WorkSchedule WorkSchedule { get; set; }
    }
}
