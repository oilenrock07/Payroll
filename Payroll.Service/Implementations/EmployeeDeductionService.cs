using System.Linq;
using Payroll.Service.Interfaces;
using System.Collections.Generic;
using Payroll.Entities.Payroll;
using Payroll.Repository.Interface;

namespace Payroll.Service.Implementations
{
    public class EmployeeDeductionService : IEmployeeDeductionService
    {
        private IEmployeeDeductionRepository _employeeDeductionRepository;

        public EmployeeDeductionService(IEmployeeDeductionRepository employeeDeductionRepository)
        {
           _employeeDeductionRepository = employeeDeductionRepository;
        }

        public EmployeeDeduction GetByDeductionAndEmployee(int deductionId, int employeeId)
        {
            return _employeeDeductionRepository.GetByDeductionAndEmployee(deductionId, employeeId);
        }

        public IEnumerable<EmployeeDeduction> GetEmployeeDeduction(int employeeId)
        {
            return _employeeDeductionRepository.Find(x => x.EmployeeId == employeeId && x.IsActive).ToList();
        }

        public virtual void UpdateEmployeeDeduction(IEnumerable<EmployeeDeduction> employeeDeductions, int employeeId)
        {
            var activeDeductions = GetEmployeeDeduction(employeeId).ToList();
            var existingDeductions = activeDeductions.Where(x => employeeDeductions.Select(y => y.DeductionId).Contains(x.DeductionId)).ToList();
            
            foreach (var employeeDeduction in employeeDeductions)
            {
                //if existing update the amount
                var existing = existingDeductions.FirstOrDefault(x => x.DeductionId == employeeDeduction.DeductionId);
                if (existing != null)
                {
                    //amount has been changed
                    if (existing.Amount != employeeDeduction.Amount)
                    {
                        _employeeDeductionRepository.Update(existing);
                        existing.Amount = employeeDeduction.Amount;
                    }
                }
                else
                {
                    employeeDeduction.EmployeeId = employeeId;
                    _employeeDeductionRepository.Add(employeeDeduction);
                }
            }

        }
    }
}
