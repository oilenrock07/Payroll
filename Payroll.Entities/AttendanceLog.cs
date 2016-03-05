using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Payroll.Entities.Enums;

namespace Payroll.Entities
{
    [Table("attendance_log")]
    public class AttendanceLog
    {
        [Key]
        public int AttendanceLogId { get; set; }

        public int EmployeeId { get; set; }
        public DateTime ClockInOut { get; set; }
        public AttendanceType Type { get; set; }
        public Boolean IsRecorded { get; set; }
    }
}
