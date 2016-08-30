using System.Linq;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Interface;
using System.Collections.Generic;
using System;
using CacheManager.Core;

namespace Payroll.Repository.Repositories
{
    public class EmployeeInfoRepository : Repository<EmployeeInfo>, IEmployeeInfoRepository
    {

        private ICacheManager<object> _cacheManager;

        public EmployeeInfoRepository(IDatabaseFactory databaseFactory, ICacheManager<object> cacheManager)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().EmployeeInfos;
            _cacheManager = cacheManager;
        }

        public EmployeeInfo GetByEmployeeId(int employeeId)
        {
            var cachedEmployee = _cacheManager.Get("emp" + employeeId);
            if (cachedEmployee == null)
            {
                var employee = Find(x => x.Employee.EmployeeId == employeeId).FirstOrDefault();
                _cacheManager.Add("emp" + employeeId, employee);

                return employee;
            }

            return cachedEmployee as EmployeeInfo;
        }

        public IList<EmployeeInfo> GetActiveByPaymentFrequency(int paymentFrequencyId)
        {
            return Find(e => e.Employee.IsActive /*&& e.PaymentFrequencyId == paymentFrequencyId*/).ToList();
        }

        public IList<EmployeeInfo> GetAllActive() {
            return Find(e => e.Employee.IsActive).ToList();
        }

        public IList<EmployeeInfo> GetAllWithAllowance()
        {
            return Find(e => e.IsActive && e.Allowance > 0).ToList();
        }
    }
}
