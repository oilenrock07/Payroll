using System.Collections.Generic;
using System.Linq;
using Payroll.Entities;
using Payroll.Repository.Interface;
using Payroll.Repository.Models.Employee;
using Payroll.Service.Interfaces;

namespace Payroll.Service.Implementations
{
    public class EmployeeMachineService : IEmployeeMachineService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeMachineRepository _employeeMachineRepository;

        public EmployeeMachineService(IEmployeeMachineRepository employeeMachineRepository, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _employeeMachineRepository = employeeMachineRepository;
        }

        public virtual IEnumerable<EmployeeMachineDao> GetEmployees(int machineId)
        {
            var employees = _employeeRepository.GetAll();
            var employeeMachines = _employeeMachineRepository.Find(x => x.Machine.IsActive && x.MachineId == machineId && x.IsActive);

            var query = from employee in employees
                        join empMachine in employeeMachines on employee.EmployeeId equals empMachine.EmployeeId into result
                        from subEmpMachine in result.DefaultIfEmpty()
                        where employee.IsActive
                        select new EmployeeMachineDao
                        {
                            EmployeeCode = employee.EmployeeCode,
                            EmployeeId = employee.EmployeeId,
                            FirstName = employee.FirstName,
                            LastName = employee.LastName,
                            MiddleName = employee.MiddleName,
                            NickName = employee.NickName,
                            Enrolled = subEmpMachine != null
                        };
            
            return query.ToList();
        }

        public virtual IEnumerable<EmployeeMachineDao> GetEmployeesNotRegistered(int machineId)
        {
            var employees = _employeeRepository.GetAll();
            var employeeMachines = _employeeMachineRepository.Find(x => x.Machine.IsActive && x.MachineId == machineId);

            var query = from employee in employees
                        join empMachine in employeeMachines on employee.EmployeeId equals empMachine.EmployeeId into result
                        from subEmpMachine in result.DefaultIfEmpty()
                        where employee.IsActive && subEmpMachine == null
                        select new EmployeeMachineDao
                        {
                            EmployeeCode = employee.EmployeeCode,
                            EmployeeId = employee.EmployeeId,
                            FirstName = employee.FirstName,
                            LastName = employee.LastName,
                            MiddleName = employee.MiddleName,
                            NickName = employee.NickName,
                            Enrolled = false
                        };

            return query.ToList();
        }
    }
}
