using System;
using System.Collections.Generic;
using Payroll.Entities.Payroll;

namespace Payroll.Models.Employee
{
    public class EmployeeLeaveViewModel
    {
        public DateTime Date { get; set; }
        public IEnumerable<EmployeeLeave> Employees { get; set; }
    }
}