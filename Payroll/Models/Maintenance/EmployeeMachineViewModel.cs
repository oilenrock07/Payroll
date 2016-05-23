using System.Collections.Generic;
using Payroll.Repository.Models.Employee;
using Payroll.Service.Interfaces.Model;

namespace Payroll.Models.Maintenance
{
    public class EmployeeMachineViewModel
    {
        public int MachineNumber { get; set; }
        public IEnumerable<EmployeeMachineDao> Employees { get; set; }
        public IPaginationModel Pagination { get; set; }
    }
}