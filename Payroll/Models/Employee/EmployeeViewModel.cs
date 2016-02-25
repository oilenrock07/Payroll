using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Payroll.Models.Employee
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }

        [StringLength(250)]
        public string EmployeeCode { get; set; }

        [StringLength(50)]
        [Required]
        public string FirstName { get; set; }

        [StringLength(50)]
        [Required]
        public string LastName { get; set; }

        [StringLength(50)]
        public string MiddleName { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public DateTime BirthDate { get; set; }

        public string DisplayBirthDate
        {
            get { return BirthDate.ToString("MM/dd/yyyy"); }
            set { BirthDate = Convert.ToDateTime(value); }
        }

        public string Picture { get; set; }

        public string FullName
        {
            get { return String.Format("{0} {1}", FirstName, LastName); }
        }
    }
}