
using System.Collections;
using System.Collections.Generic;
using Payroll.Entities;
using Payroll.Entities.Payroll;

namespace Payroll.Models.Attendance
{
    public class CreateHoursPerCompanyViewModel
    {
        public string ModalTitle { get; set; }
        public IEnumerable<Company> Companies { get; set; }
        public EmployeeTotalHoursViewModel EmployeeTotalHoursViewModel { get; set; }
        public IEnumerable<TotalEmployeeHoursPerCompany> RegularHoursPerCompany { get; set; }
        public IEnumerable<TotalEmployeeHoursPerCompany> OvertimePerCompany { get; set; }
        public IEnumerable<TotalEmployeeHoursPerCompany> NightDifferentialPerCompany { get; set; }
    }
}