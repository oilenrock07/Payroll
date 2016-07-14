using System;
using System.Collections.Generic;
using Payroll.Repository.Interface;
using Payroll.Service.Interfaces;
using System.Linq;
using Payroll.Entities.Payroll;
using Payroll.Repository.Models.Payroll;

namespace Payroll.Service.Implementations
{
    public class EmployeeAdjustmentService : IEmployeeAdjustmentService
    {
        private readonly IEmployeeAdjustmentRepository _employeeAdjustmentRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeAdjustmentService(IEmployeeAdjustmentRepository employeeAdjustmentRepository, IEmployeeRepository employeeRepository)
        {
            _employeeAdjustmentRepository = employeeAdjustmentRepository;
            _employeeRepository = employeeRepository;
        }

        public IEnumerable<EmployeeAdjustmentDao> GetEmployeeAdjustmentByDate(DateTime startDate, DateTime endDate)
        {
            var employees = _employeeRepository.GetAllActive();
            var employeeAdjustments = _employeeAdjustmentRepository.Find(x => x.IsActive && x.Date >= startDate && x.Date <= endDate);

            var query = from employee in employees
                        join empAdjustment in employeeAdjustments on employee.EmployeeId equals empAdjustment.EmployeeId into result
                        from subEmpAdjustment in result.DefaultIfEmpty()
                        group subEmpAdjustment by employee into grouped
                        select new EmployeeAdjustmentDao
                        {
                            Employee = grouped.Key,
                            EmployeeAdjustmentCount = grouped.Count(x => x != null)
                        };

            return query;
        }

        public IEnumerable<EmployeeAdjustment> GetEmployeeAdjustments(int employeeId, DateTime startDate, DateTime endDate)
        {
            var employeeAdjustments =
                _employeeAdjustmentRepository.Find(
                    x => x.IsActive && x.EmployeeId == employeeId && x.Date >= startDate && x.Date <= endDate);

            return employeeAdjustments;
        }

        public void Update(EmployeeAdjustment employeeAdjustment)
        {
            _employeeAdjustmentRepository.Update(employeeAdjustment);
        }
    }
}
