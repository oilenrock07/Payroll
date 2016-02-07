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

        [StringLength(50)]
        public string EmployeeCode { get; set; }
        public DateTime ClockInOut { get; set; }
        public AttendanceType type { get; set; }
    }
}
