using Payroll.Entities.Enums;
using Payroll.Infrastructure.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("work_schedule")]
    public class WorkSchedule : BaseEntity
    {
        [Key]
        public int WorkScheduleId { get; set; }

        [StringLength(250)]
        public string WorkScheduleName { get; set; }

        public TimeSpan TimeStart { get; set; }

        public TimeSpan TimeEnd { get; set; }

        public int WeekStart { get; set; }

        public int WeekEnd { get; set; }
    }
}
