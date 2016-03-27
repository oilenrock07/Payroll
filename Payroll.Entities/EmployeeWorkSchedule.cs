using Payroll.Infrastructure.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("employee_workschedule")]
    public class EmployeeWorkSchedule : BaseEntity
    {
        [Key]
        public int EmployeeWorkScheduleId { get; set; }

        public int EmployeeId { get; set; }

        [ForeignKey("WorkSchedule")]
        public int WorkScheduleId { get; set; }
        public virtual WorkSchedule WorkSchedule { get; set; }
    }
}
