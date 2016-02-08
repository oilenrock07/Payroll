using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using System.Collections.Generic;

namespace Payroll.Repository.Interface
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Employee GetByCode(string code);

        IList<Employee> GetActiveByPaymentFrequency(int PaymentFrequencyId);
    }
}
