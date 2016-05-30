using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;

namespace Payroll.Entities
{
    [Table("attendance")]
    public class Attendance : BaseEntity
    {
        public Attendance()
        {
            IsHoursCounted = false;
        }
        [Key]
        public int AttendanceId { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        public DateTime ClockIn { get; set; }

        public DateTime ? ClockOut { get; set; }

        public bool IsHoursCounted { get; set; }

        public bool ? IsManuallyEdited { get; set; }
    }
}
