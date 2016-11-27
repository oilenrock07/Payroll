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
    public class TotalEmployeeHoursRepository : Repository<TotalEmployeeHours>, ITotalEmployeeHoursRepository
    {
        public TotalEmployeeHoursRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().TotalEmployeeHours;
        }

        public TotalEmployeeHours GetByEmployeeDateAndType(int employeeId, DateTime date, RateType type)
        {
            return Find(eh => eh.IsActive && eh.EmployeeId == employeeId && eh.Date == date && eh.Type == type).FirstOrDefault();
        }

        public IList<TotalEmployeeHours> GetByEmployeeDate(int employeeId, DateTime date)
        {
            return Find(eh => eh.IsActive && eh.EmployeeId == employeeId && eh.Date == date).ToList();
        }

        public IList<TotalEmployeeHours> GetByDateRange(DateTime dateFrom, DateTime dateTo)
        {
            return Find(eh => eh.IsActive && eh.Date >= dateFrom && eh.Date < dateTo)
                .OrderByDescending(eh => eh.Date).ThenBy(eh => eh.EmployeeId).ThenBy(eh => eh.Type).ToList();
        }

        public IList<TotalEmployeeHours> GetByTypeAndDateRange(int employeeId, RateType rateType, DateTime payrollStartDate, DateTime payrollEndDate)
        {
            return Find(eh => eh.IsActive && eh.EmployeeId == employeeId &&
                eh.Type == rateType && eh.Date >= payrollStartDate && eh.Date < payrollEndDate)
                    .OrderByDescending(eh => eh.Date).ToList();
        }

        public IList<TotalEmployeeHours> GetByDateRange(int employeeId, DateTime payrollStartDate, DateTime payrollEndDate)
        {
            return Find(eh => eh.IsActive && eh.EmployeeId == employeeId &&
                eh.Date >= payrollStartDate && eh.Date < payrollEndDate)
                    .OrderByDescending(eh => eh.Date).ToList();
        }

        public double CountTotalHours(int employeeId, DateTime date)
        {
            return Find(eh => eh.IsActive && eh.Date == date && eh.EmployeeId == employeeId &&
                (eh.Type == RateType.Regular || eh.Type == RateType.OverTime)).ToList().Sum(eh => eh.Hours);
        }
    }
}
