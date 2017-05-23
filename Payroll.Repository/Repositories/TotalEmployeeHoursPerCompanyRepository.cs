using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Util;
using Payroll.Entities.Payroll;
using Payroll.Entities.Users;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;

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
    }
}
