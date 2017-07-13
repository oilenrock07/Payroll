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

        public IList<EmployeeHours> GetByDateRange(DateTime fromDate, DateTime toDate)
        {
            return Find(eh => eh.IsActive && eh.Date >= fromDate && eh.Date < toDate).ToList();
        }

        public IList<EmployeeHours> GetByEmployeeAndDateRange(int employeeId, DateTime dateFrom, DateTime dateTo)
        {
            return Find(eh => eh.IsActive && eh.EmployeeId == employeeId &&
                eh.Date >= dateFrom && eh.Date < dateTo)
                   .OrderBy(eh => eh.Date).ThenBy(eh => eh.EmployeeHoursId).ToList();
        }

        public IList<EmployeeHours> GetForProcessingByDateRange(bool isManual, DateTime fromDate, DateTime toDate)
        {
            if (isManual)
            {
                return Find(eh => eh.IsActive && eh.Date >= fromDate
                    && eh.Date < toDate).OrderByDescending(eh => eh.Date).ThenBy(eh => eh.EmployeeId).ThenBy(eh => eh.Type).ToList();
            }
            return Find(eh => eh.IsActive && !eh.IsIncludedInTotal && eh.Date >= fromDate 
                && eh.Date < toDate).OrderByDescending(eh => eh.Date).ThenBy(eh => eh.EmployeeId).ThenBy(eh => eh.Type).ToList();
        }

        public IList<EmployeeHours> GetForProcessingByEmployeeAndDate(int employeeId, DateTime fromDate, DateTime toDate)
        {
            return Find(eh => eh.IsActive && !eh.IsIncludedInTotal && eh.EmployeeId == employeeId && eh.Date >= fromDate 
                && eh.Date < toDate).OrderByDescending(eh => eh.Date).ThenBy(eh => eh.EmployeeId).ThenBy(eh => eh.Type).ToList();
        }
    }
}
