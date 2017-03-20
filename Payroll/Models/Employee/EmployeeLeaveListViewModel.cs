using System.Collections.Generic;
using System.Web.Mvc;
using Payroll.Entities.Payroll;

namespace Payroll.Models.Employee
{
    public class EmployeeLeaveListViewModel
    {
        public IEnumerable<EmployeeLeave> EmployeeLeaves { get; set; }
        //public IEnumerable<EmployeeLeaveViewModel> Employees { get; set; }

        public int Month { get; set; }
        public IEnumerable<SelectListItem> Months { get; set; }

        public int Year { get; set; }
        public IEnumerable<SelectListItem> Years { get; set; }
    }
}