using System;
using System.Collections.Generic;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Infrastructure.Implementations;

namespace Payroll.Repository.Repositories
{
    public class EmployeeLeaveRepository : Repository<EmployeeLeave>, IEmployeeLeaveRepository
    {
        public EmployeeLeaveRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().EmployeeLeaves;
        }

        public IEnumerable<EmployeeLeave> GetEmployeeLeavesByDate(int month, int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            return Find(x => x.IsActive && x.Date >= startDate && x.Date <= endDate);
        }
    }
}
