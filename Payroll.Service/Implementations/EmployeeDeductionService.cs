using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
