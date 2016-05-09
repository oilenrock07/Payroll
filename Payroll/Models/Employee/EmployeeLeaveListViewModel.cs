using System.Collections.Generic;
using System.Web.Mvc;

namespace Payroll.Models.Employee
{
    public class EmployeeLeaveListViewModel
    {
        public IEnumerable<EmployeeLeaveViewModel> Employees { get; set; }

        public int Month { get; set; }
        public IEnumerable<SelectListItem> Months { get; set; }

        public int Year { get; set; }
        public IEnumerable<SelectListItem> Years { get; set; }
    }
}