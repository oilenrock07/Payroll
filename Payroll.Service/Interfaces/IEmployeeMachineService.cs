using System.Collections.Generic;
using Payroll.Repository.Models.Employee;

namespace Payroll.Service.Interfaces
{
    public interface IEmployeeMachineService
    {
        IEnumerable<EmployeeMachineDao> GetEmployees(string ipAddress);
    }
}
