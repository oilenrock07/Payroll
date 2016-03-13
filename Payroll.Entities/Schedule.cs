using Payroll.Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("schedule")]
    public class Schedule
    {
        [Key]
        public int ScheduleId { get; set; }

        public int StartDay { get; set; }

        public int EndDay { get; set; }

        public TimePeriod Timeperiod { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
