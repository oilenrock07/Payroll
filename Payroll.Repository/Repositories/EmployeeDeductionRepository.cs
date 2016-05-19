using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using System.Linq;

namespace Payroll.Repository.Repositories
{
    public class EmployeeDeductionRepository : Repository<EmployeeDeduction>, IEmployeeDeductionRepository
    {
        public EmployeeDeductionRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().EmployeeDeductions;
        }

        public EmployeeDeduction GetByDeductionAndEmployee(int deductionId, int employeeId)
        {
            return Find(ed => ed.IsActive && ed.DeductionId == deductionId &&
                ed.EmployeeId == employeeId).FirstOrDefault();
        }
    }
}
