using Payroll.Service.Interfaces;
using Payroll.Entities;
using Payroll.Repository.Interface;

namespace Payroll.Service.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public Employee GetById(int id)
        {
            return _employeeRepository.GetById(id);
        }
    }
}
