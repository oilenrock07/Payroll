using System;
using System.Collections.Generic;
using Payroll.Entities.Payroll;

namespace Payroll.Repository.Models
{
    public class HoursPerCompanyDao
    {
        public DateTime Date { get; set; }
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public IEnumerable<TotalEmployeeHours> TotalEmployeeHours { get; set; }
    }
}
