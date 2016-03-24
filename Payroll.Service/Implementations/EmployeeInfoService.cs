using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payroll.Entities;
using Payroll.Repository.Interface;

namespace Payroll.Service.Implementations
{
    public class EmployeeInfoService : IEmployeeInfoService
    {
        private readonly IEmployeeInfoRepository _employeeInfoRepository;

        public EmployeeInfo GetByEmployeeId(int employeeId)
        {
         return _employeeInfoRepository.GetByEmployeeId(employeeId);
        }

        public IList<EmployeeInfo> GetActiveByPaymentFrequency(int PaymentFrequencyId)
        {
            return _employeeInfoRepository.GetActiveByPaymentFrequency(PaymentFrequencyId);
        }
    }
}
