using Payroll.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Entities
{
    [Table("tbl_schedule")]
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
