using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payroll.Entities.Enums;

namespace Payroll.Repository.Repositories
{
    public class EmployeeDailyPayrollRepository : Repository<EmployeeDailyPayroll>, IEmployeeDailyPayrollRepository
    {
        public EmployeeDailyPayrollRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
            DbSet = databaseFactory.GetContext().EmployeeDailyPayroll;
        }

        public EmployeeDailyPayroll GetByDate(int employeeId, DateTime date)
        {
            return Find(p => p.IsActive && p.EmployeeId == employeeId
                    && p.Date == date).FirstOrDefault();
        }

        public IList<EmployeeDailyPayroll> GetByDateRange(DateTime dateFrom, DateTime dateTo)
        {
            return Find(p => p.IsActive && p.Date >= dateFrom && p.Date < dateTo)
                .OrderBy(p => p.EmployeeId).ThenByDescending(p => p.Date).ToList();
        }

        public IList<EmployeeDailyPayroll> GetByTypeAndDateRange(RateType rateType, DateTime dateFrom, DateTime dateTo)
        {
            return Find(p => p.IsActive && p.RateType == rateType
                && p.Date >= dateFrom && p.Date < dateTo).OrderByDescending(p => p.Date).ToList();
        }
    }
}
