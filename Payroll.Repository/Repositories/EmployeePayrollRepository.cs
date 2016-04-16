using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Repository.Repositories
{
    public class EmployeePayrollRepository : Repository<EmployeePayroll>, IEmployeePayrollRepository
    {
        public EmployeePayrollRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().EmployeePayroll;
        }

    }
}
