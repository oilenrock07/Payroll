using Payroll.Repository.Interface;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Payroll.Repository.Repositories
{
    public class EmployeeHoursRepository : Repository<EmployeeHours>, IEmployeeHoursRepository
    {
        public EmployeeHoursRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().EmployeeHours;
        }

        public IList<EmployeeHours> GetByEmployeeAndDateRange(int employeeId, DateTime dateFrom, DateTime dateTo)
        {
            return Find(eh => eh.EmployeeId == employeeId &&
                eh.Date >= dateFrom && eh.Date <= dateTo)
                   .OrderByDescending(eh => eh.Date).ToList();
        }
    }
}
