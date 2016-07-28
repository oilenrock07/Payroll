using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Payroll.Entities;
using Payroll.Entities.Payroll;
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
        [Required]
        public int PaymentFrequency { get; set; }
        [Required]
        public int WorkScheduleId { get; set; }
        public int Gender { get; set; }
        public int EmploymentStatus { get; set; }
        public string CheckedDepartments { get; set; }
        public string CheckedEmployeeDeductions { get; set; }
        public bool IsPrivate { get; set; }


        public WorkSchedule WorkSchedule { get; set; }
        public EmployeeInfo EmployeeInfo { get; set; }
        public IEnumerable<SelectListItem> Positions { get; set; }
        public IEnumerable<SelectListItem> PaymentFrequencies { get; set; }
        public IEnumerable<SelectListItem> Genders { get; set; }
        public IEnumerable<SelectListItem> EmploymentStatuses { get; set; }
        public IEnumerable<EmployeeDepartmentViewModel> Departments { get; set; }
        public IEnumerable<EmployeeDeductionViewModel> EmployeeDeductions { get; set; }
        

        [Required(ErrorMessageResourceType = typeof (ErrorMessages), ErrorMessageResourceName = "REQUIRED_BIRTHDATE")]
        public string DisplayBirthDate
        {
            get { return EmployeeInfo.Employee.BirthDate.ToString("MM/dd/yyyy"); }
            set { EmployeeInfo.Employee.BirthDate = Convert.ToDateTime(value); }
        }
    }
}