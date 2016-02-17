using System;
using Payroll.Common.Enums;

namespace Payroll.LoginDisplay.Models.Payroll
{
    public class LogInViewModel
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Datetime { get; set; }
        public string ImagePath { get; set; }
        public AttendanceCode AttendanceCode { get; set; }

        public string FullName
        {
            get { return String.Format("{0} {1}", FirstName, LastName); }
        }
    }
}