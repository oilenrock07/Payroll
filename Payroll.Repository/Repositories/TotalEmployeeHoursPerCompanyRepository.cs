using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Util;
using Payroll.Entities.Payroll;
using Payroll.Entities.Users;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Entities.Enums;

namespace Payroll.Repository.Repositories
{
    public class TotalEmployeeHoursPerCompanyRepository : Repository<TotalEmployeeHoursPerCompany>, ITotalEmployeeHoursPerCompanyRepository
    {
        public TotalEmployeeHoursPerCompanyRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            
        }

        public virtual void DeleteByTotalEmployeeHoursPerCompanyIds(IEnumerable<int> ids)
        {
            if (EnumerableExtensions.Any(ids))
            {
                foreach (var id in ids)
                {
                    var item = GetById(id);
                    PermanentDelete(item);
                }
            }
        }

        //public virtual void DeleteByTotalEmployeeHoursPerCompanyIds(IEnumerable<int> ids)
        //{
        //    if (EnumerableExtensions.Any(ids))
        //    {
        //        var delimitedIds = String.Join(",", ids);
        //        ExecuteSqlCommandTransaction(String.Format("DELETE FROM employee_hours_total_per_company WHERE TotalEmployeeHoursPerCompanyId IN ({0})", delimitedIds));
        //    }
        //}

        public TotalEmployeeHoursPerCompany GetByEmployeeDateAndType(int employeeId, DateTime date, RateType type)
        {
            return Find(eh => eh.IsActive && eh.TotalEmployeeHours.EmployeeId == employeeId && eh.TotalEmployeeHours.Date == date && eh.TotalEmployeeHours.Type == type).FirstOrDefault();
        }

        public IList<TotalEmployeeHoursPerCompany> GetByEmployeeDate(int employeeId, DateTime date)
        {
            return Find(eh => eh.IsActive && eh.TotalEmployeeHours.EmployeeId == employeeId && eh.TotalEmployeeHours.Date == date).ToList();
        }

        public IList<TotalEmployeeHoursPerCompany> GetByDateRange(DateTime dateFrom, DateTime dateTo)
        {
            return Find(eh => eh.IsActive && eh.TotalEmployeeHours.Date >= dateFrom && eh.TotalEmployeeHours.Date < dateTo)
                .OrderByDescending(eh => eh.TotalEmployeeHours.Date).ThenBy(eh => eh.TotalEmployeeHours.EmployeeId).ThenBy(eh => eh.TotalEmployeeHours.Type).ToList();
        }

        public IList<TotalEmployeeHoursPerCompany> GetByDateRange(DateTime dateFrom, DateTime dateTo, int employeeId)
        {
            return Find(eh => eh.IsActive && eh.TotalEmployeeHours.Date >= dateFrom && eh.TotalEmployeeHours.Date < dateTo && eh.TotalEmployeeHours.EmployeeId == employeeId)
                .OrderByDescending(eh => eh.TotalEmployeeHours.Date).ThenBy(eh => eh.TotalEmployeeHours.Type).ToList();
        }

        public IList<TotalEmployeeHoursPerCompany> GetByTypeAndDateRange(int employeeId, RateType rateType, DateTime payrollStartDate, DateTime payrollEndDate)
        {
            return Find(eh => eh.IsActive && eh.TotalEmployeeHours.EmployeeId == employeeId &&
                eh.TotalEmployeeHours.Type == rateType && eh.TotalEmployeeHours.Date >= payrollStartDate && eh.TotalEmployeeHours.Date < payrollEndDate)
                    .OrderByDescending(eh => eh.TotalEmployeeHours.Date).ToList();
        }

        public IList<TotalEmployeeHoursPerCompany> GetByDateRange(int employeeId, DateTime payrollStartDate, DateTime payrollEndDate)
        {
            return Find(eh => eh.IsActive && eh.TotalEmployeeHours.EmployeeId == employeeId &&
                eh.TotalEmployeeHours.Date >= payrollStartDate && eh.TotalEmployeeHours.Date < payrollEndDate)
                    .OrderByDescending(eh => eh.TotalEmployeeHours.Date).ToList();
        }

        public double CountTotalHours(int employeeId, DateTime date)
        {
            return Find(eh => eh.IsActive && eh.TotalEmployeeHours.Date == date && eh.TotalEmployeeHours.EmployeeId == employeeId &&
                (eh.TotalEmployeeHours.Type == RateType.Regular || eh.TotalEmployeeHours.Type == RateType.OverTime)).ToList().Sum(eh => eh.Hours);
        }
    }
}
