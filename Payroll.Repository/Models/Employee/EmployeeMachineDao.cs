
using System;

namespace Payroll.Repository.Models.Employee
{
    public class EmployeeMachineDao
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string NickName { get; set; }
        public bool Enrolled { get; set; }
    }
}
