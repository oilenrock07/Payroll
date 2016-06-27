using System.Collections.Generic;
using System.Web.Mvc;
using Payroll.Repository.Models.Payroll;
using Payroll.Service.Interfaces.Model;

namespace Payroll.Models.Payroll
{
    public class PayrollViewModel
    {
        public string PayrollDate { get; set; }
        public IEnumerable<SelectListItem> PayrollDates { get; set; }
        public IPaginationModel Pagination { get; set; }
        public IEnumerable<PayrollDao> Payrolls { get; set; }
    }
}