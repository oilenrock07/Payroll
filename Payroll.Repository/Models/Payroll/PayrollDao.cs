using System;

namespace Payroll.Repository.Models.Payroll
{
    public class PayrollDao
    {
        public int PayrollId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }

        public decimal TotalNet { get; set; }
        public decimal TotalDeduction { get; set; }
        public decimal TotalGross { get; set; }

        public string FullName
        {
            get { return String.Format("{0}, {1}, {2}", LastName, FirstName, MiddleName); }
        }

        //do we still need this? they will all have the same payroll
        //public DateTime PayrollDate { get; set; }
    }
}
