using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Payroll.Entities.Enums;
using Payroll.Infrastructure.Entities;

namespace Payroll.Entities
{
    [Table("attendance_log")]
    public class AttendanceLog : BaseEntity
    {
        [Key]
        public int AttendanceLogId { get; set; }

        public int EmployeeId { get; set; }
        public DateTime ClockInOut { get; set; }
        public AttendanceType Type { get; set; }
        public bool IsRecorded { get; set; }
        public bool IsConsidered { get; set; }
    }
}
