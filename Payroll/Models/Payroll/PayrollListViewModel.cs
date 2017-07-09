using Payroll.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Payroll.Models.Payroll
{
    public class PayrollListViewModel
    {
        public int PayrollId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }

        public decimal TotalNet { get; set; }
        public decimal TotalDeduction { get; set; }
        public decimal TotalGross { get; set; }

        public Company Company { get; set; }

        public string FullName
        {
            get { return String.Format("{0}, {1}, {2}", LastName, FirstName, MiddleName); }
        }
    }
}