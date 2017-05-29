using Payroll.Entities.Enums;
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
    public class EmployeePayrollItemPerCompanyRepository : Repository<EmployeePayrollItemPerCompany>, IEmployeePayrollItemPerCompanyRepository
    {
        public EmployeePayrollItemPerCompanyRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
            DbSet = databaseFactory.GetContext().EmployeePayrollItemsPerCompany;
        }

        public IList<EmployeePayrollItemPerCompany> GetByDateRange(DateTime dateFrom, DateTime dateTo)
        {
            return Find(ep => ep.IsActive && ep.PayrollDate >= dateFrom && ep.PayrollDate < dateTo)
              .OrderByDescending(ep => ep.PayrollDate).ThenBy(ep => ep.EmployeeId).ToList();
        }

        public EmployeePayrollItemPerCompany Find(int employeeId, DateTime date, RateType rateType)
        {
            return Find(ep => ep.IsActive && ep.PayrollId == null
                && ep.EmployeeId == employeeId && ep.PayrollDate == date
                && ep.RateType == rateType).FirstOrDefault();
        }
    }
}
