using System.Collections.Generic;
using System.Linq;
using Payroll.Common.Extension;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Repository.Models;

namespace Payroll.Repository.Repositories
{
    public class EmployeeLoanRepository : Repository<EmployeeLoan>, IEmployeeLoanRepository
    {
        public EmployeeLoanRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().EmployeeLoans;
        }

        public virtual IEnumerable<EmployeeLoanDao> GetActiveEmployeeLoans()
        {
            var result = Find(x => x.IsActive);
            return result.MapCollection<EmployeeLoan, EmployeeLoanDao>((s, d) =>
            {
                d.FirstName = s.Employee.FirstName;
                d.LastName = s.Employee.LastName;
                d.LoanName = s.Loan.LoanName;
            }).ToList();
        }
    }
}
