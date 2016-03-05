using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Payroll.Entities;
using Payroll.Resources;

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
        public int EmploymentStatus { get; set; }
        public string CheckedDepartments { get; set; }
        public bool IsPrivate { get; set; }


        public EmployeeInfo EmployeeInfo { get; set; }
        public IEnumerable<SelectListItem> Positions { get; set; }
        public IEnumerable<SelectListItem> PaymentFrequencies { get; set; }
        public IEnumerable<SelectListItem> Genders { get; set; }
        public IEnumerable<SelectListItem> EmploymentStatuses { get; set; }
        public IEnumerable<EmployeeDepartmentViewModel> Departments { get; set; }

        [Required(ErrorMessageResourceType = typeof (ErrorMessages), ErrorMessageResourceName = "REQUIRED_BIRTHDATE")]
        public string DisplayBirthDate
        {
            get { return EmployeeInfo.Employee.BirthDate.ToString("MM/dd/yyyy"); }
            set { EmployeeInfo.Employee.BirthDate = Convert.ToDateTime(value); }
        }
    }
}