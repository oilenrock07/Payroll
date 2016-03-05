using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Payroll.Entities;

namespace Payroll.Models.Employee
{
    public class EmployeeInfoViewModel
    {
        public EmployeeInfoViewModel()
        {
            EmployeeInfo = new EmployeeInfo();
        }

        public string ImagePath { get; set; }
        public int PositionId { get; set; }
        public int PaymentFrequency { get; set; }
        public int Gender { get; set; }
        public string CheckedDepartments { get; set; }
        public bool IsPrivate { get; set; }


        public EmployeeInfo EmployeeInfo { get; set; }
        public IEnumerable<SelectListItem> Positions { get; set; }
        public IEnumerable<SelectListItem> PaymentFrequencies { get; set; }
        public IEnumerable<SelectListItem> Genders { get; set; }
        public IEnumerable<EmployeeDepartmentViewModel> Departments { get; set; }

        public string DisplayBirthDate
        {
            get { return EmployeeInfo.Employee.BirthDate.ToString("MM/dd/yyyy"); }
            set { EmployeeInfo.Employee.BirthDate = Convert.ToDateTime(value); }
        }
    }
}