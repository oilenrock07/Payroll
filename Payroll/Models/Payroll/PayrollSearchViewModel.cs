using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Payroll.Service.Interfaces.Model;

namespace Payroll.Models.Payroll
{
    public class PayrollSearchViewModel
    {
        public int EmployeeId { get; set; }
        public int CompanyId { get; set; }
        public string EmployeeName { get; set; }
        public string CompanyName { get; set; }

        public string Date { get; set; }
        public IEnumerable<SelectListItem> PayrollDates { get; set; }
        public IEnumerable<PayrollListViewModel> Payrolls { get; set; }

        public IPaginationModel Pagination { get; set; }
    }
}