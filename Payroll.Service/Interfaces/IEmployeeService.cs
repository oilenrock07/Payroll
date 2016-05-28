using Payroll.Entities;
using System.Collections.Generic;
using Payroll.Repository.Models.Employee;

namespace Payroll.Service.Interfaces
{
    public interface IEmployeeService
    {
        Employee GetById(int id);
        IEnumerable<EmployeeNames> SearchEmployee(string criteria);
    }
}
