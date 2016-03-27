using Payroll.Infrastructure.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("attendance")]
    public class Attendance : BaseEntity
    {
        [Key]
        public int AttendanceId { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        public DateTime ClockIn { get; set; }

        public DateTime ? ClockOut { get; set; }

        public bool ? IsManuallyEdited { get; set; }
    }
}
