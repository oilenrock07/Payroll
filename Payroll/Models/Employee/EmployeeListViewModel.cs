using System.Collections.Generic;
using Payroll.Entities;
using Payroll.Service.Interfaces.Model;

namespace Payroll.Models.Employee
{
    public class EmployeeListViewModel
    {
        public IPaginationModel Pagination { get; set; }
        public IEnumerable<EmployeeInfo> Employees { get; set; }
    }
}