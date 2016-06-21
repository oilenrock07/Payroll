using System.ComponentModel.DataAnnotations;
using Payroll.Entities.Base;
using Payroll.Entities.Enums;

namespace Payroll.Entities
{
    public class SchedulerLog : BaseEntity
    {
        [Key]
        public int SchedulerLogId { get; set; }
        public string ScheduleType { get; set; }
        public SchedulerLogType LogType { get; set; }
        public string Exception { get; set; }
    }
}
