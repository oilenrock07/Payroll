using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("tbl_work_schedule")]
    public class WorkSchedule
    {
        [Key]
        public int WorkScheduleId { get; set; }

        [StringLength(250)]
        public string WorkScheduleName { get; set; }

        public int TimeStart { get; set; }

        public int TimeEnd { get; set; }

        public int WeekStart { get; set; }

        public int WeekEnd { get; set; }
    }
}
