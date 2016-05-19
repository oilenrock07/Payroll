using Payroll.Entities.Enums;
using System;

namespace Payroll.Repository.Models
{
    public class AttendanceLogDao
    {
        public int AttendanceLogId { get; set; }

        public int EmployeeId { get; set; }
        public DateTime ClockInOut { get; set; }
        public AttendanceType Type { get; set; }
        public int MachineId { get; set; }
        public string IpAddress { get; set; }
        public bool IsRecorded { get; set; }
        public bool IsConsidered { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
    }
}
