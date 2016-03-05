using System.Linq;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Infrastructure.Implementations;
using System;
using System.Collections.Generic;

namespace Payroll.Repository.Repositories
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        private readonly IEmployeeDepartmentRepository _employeeDepartmentRepository;

        public EmployeeRepository(IDatabaseFactory databaseFactory, IEmployeeDepartmentRepository employeeDepartmentRepository)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().Employees;
            _employeeDepartmentRepository = employeeDepartmentRepository;
        }

        public IList<Employee> GetActiveByPaymentFrequency(int PaymentFrequencyId)
        {
            //TODO check the payment frequency id
            return Find(e => e.IsActive).ToList();
        }

        public Employee GetByCode(string code)
        {
            return Find(e => e.EmployeeCode == code).FirstOrDefault();
        }

        public IEnumerable<EmployeeDepartment> GetDepartments(int employeeId)
        {
            return _employeeDepartmentRepository.Find(x => x.EmployeeId == employeeId && x.IsActive);
        }

        public void UpdateDepartment(IEnumerable<int> departmentIds, int employeeId)
        {
            //get first the current departments
            var currentDepartments = GetDepartments(employeeId).Select(x => new { x.DepartmentId, x.EmployeeDepartmentId} ).ToList();
            
            //add the newly added departments
            var newDepartments = departmentIds.Except(currentDepartments.Select(x => x.DepartmentId));
            foreach (var newDepartment in newDepartments)
            {
                _employeeDepartmentRepository.Add(new EmployeeDepartment
                {
                    DepartmentId = newDepartment,
                    EmployeeId = employeeId,
                    IsActive = true
                });
            }

            //remove the removed departments
            var toRemoveDepartments = currentDepartments.Select(x=> x.DepartmentId).Except(newDepartments);
            foreach (var removeDepartment in toRemoveDepartments)
            {
                var employeeDepartmentId = currentDepartments.First(x => x.DepartmentId == removeDepartment).EmployeeDepartmentId;
                var employeeDepartment = new EmployeeDepartment {EmployeeDepartmentId = employeeDepartmentId};
                _employeeDepartmentRepository.Update(employeeDepartment);
                employeeDepartment.IsActive = false;

            }

        }

        public IEnumerable<Employee> SearchEmployee(string criteria)
        {
            //ExecuteSqlCommand("SELECT * FROM Employee WHERE FirstName LIKE '%{0}%' OR LastName LIKE '%{0}%' OR EmployeeCode LIKE '%{0}%' OR EmployeeId={0}", criteria);
            return Find(x => x.FirstName.Contains(criteria));
        }
    }
}
