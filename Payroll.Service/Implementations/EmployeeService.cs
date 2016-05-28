using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Payroll.Common.Extension;
using Payroll.Repository.Models.Employee;
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

        public IEnumerable<EmployeeNames> SearchEmployee(string criteria)
        {
            var employeeNames = new List<Employee>();
            var firstNames = _employeeRepository.Find(x => x.FirstName.Contains(criteria) && x.IsActive).ToList();
            var lastNames = _employeeRepository.Find(x => x.LastName.Contains(criteria) && x.IsActive).ToList();

            employeeNames.AddRange(firstNames);
            employeeNames.AddRange(lastNames);

            return employeeNames.Select(x => new EmployeeNames()
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                EmployeeId = x.EmployeeId
            });
        }
    }
}
