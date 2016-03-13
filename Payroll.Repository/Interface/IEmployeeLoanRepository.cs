using System.Collections.Generic;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Models;

namespace Payroll.Repository.Interface
{
    public interface IEmployeeLoanRepository : IRepository<EmployeeLoan>
    {
        IEnumerable<EmployeeLoanDao> GetActiveEmployeeLoans();
    }
}
