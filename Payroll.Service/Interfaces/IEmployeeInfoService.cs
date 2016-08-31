using Payroll.Entities;
using System.Collections.Generic;

namespace Payroll.Service.Interfaces
{
    public interface IEmployeeInfoService
    {
        EmployeeInfo GetByEmployeeId(int employeeId);

        IList<EmployeeInfo> GetActiveByPaymentFrequency(int PaymentFrequencyId);

        IList<EmployeeInfo> GetAllActive();

        IList<EmployeeInfo> GetAllWithAllowance();
    }
}
