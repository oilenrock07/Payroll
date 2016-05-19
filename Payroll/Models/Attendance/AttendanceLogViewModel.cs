
namespace Payroll.Models.Attendance
{
    public class AttendanceLogViewModel
    {
        public string FullName { get; set; }
        public string IsRecorded { get; set; }
        public string Datetime { get; set; }
        public string Type { get; set; }
        public int MachineId { get; set; }
        public string IpAddress { get; set; }
    }
}