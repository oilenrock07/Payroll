using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("tbl_attendance")]
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }

        public int EmployeeId { get; set; }
        public DateTime ClockInOut { get; set; }
        public int AttendanceType { get; set; }
    }
}
