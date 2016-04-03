using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using System.Collections.Generic;

namespace Payroll.Repository.Interface
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Employee GetByCode(string code);

        IEnumerable<EmployeeDepartment> GetDepartments(int employeeId);

        void UpdateDepartment(IEnumerable<int> departmentIds, int employeeId);

        IEnumerable<Employee> SearchEmployee(string criteria);
    }
}
