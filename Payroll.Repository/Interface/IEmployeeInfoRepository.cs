using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using System.Collections.Generic;

namespace Payroll.Repository.Interface
{
    public interface IEmployeeInfoRepository : IRepository<EmployeeInfo>
    {
        EmployeeInfo GetByEmployeeId(int employeeId);

        IList<EmployeeInfo> GetActiveByPaymentFrequency(int PaymentFrequencyId);

        IList<EmployeeInfo> GetAllActive();

        IList<EmployeeInfo> GetAllWithAllowance();

    }
}
