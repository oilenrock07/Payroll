using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payroll.Entities;
using Payroll.Repository.Interface;
using Payroll.Infrastructure.Interfaces;

namespace Payroll.Service.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeService(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
        {
            employeeRepository = _employeeRepository;
            unitOfWork = _unitOfWork;
        }

        public IList<Employee> GetActiveByPaymentFrequency(int PaymentFrequencyId)
        {
           return _employeeRepository.GetActiveByPaymentFrequency(PaymentFrequencyId);
        }
    }
}
