using System.Linq;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Interface;
using System.Collections.Generic;
using CacheManager.Core;
using Payroll.Repository.Constants;

namespace Payroll.Repository.Repositories
{
    public class EmployeeInfoRepository : Repository<EmployeeInfo>, IEmployeeInfoRepository
    {
        private readonly ICacheManager<object> _cacheManager;

        public EmployeeInfoRepository(IDatabaseFactory databaseFactory, ICacheManager<object> cacheManager = null)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().EmployeeInfos;
            _cacheManager = cacheManager;
        }

        public override void Update(EmployeeInfo entity)
        {
            base.Update(entity);

            var cachedEmployee = _cacheManager != null ? _cacheManager.Get(entity.EmployeeId.ToString(), CacheRegion.EmployeeInfo) : null;
            if (cachedEmployee != null)
                _cacheManager.Remove(entity.EmployeeId.ToString(), CacheRegion.EmployeeInfo);
        }

        public EmployeeInfo GetByEmployeeId(int employeeId)
        {
            var cachedEmployee = _cacheManager != null ? _cacheManager.Get(employeeId.ToString(), CacheRegion.EmployeeInfo) : null;
            if (cachedEmployee == null)
            {
                var employee = Find(x => x.Employee.EmployeeId == employeeId).FirstOrDefault();
                _cacheManager.Add(employeeId.ToString(), employee, CacheRegion.EmployeeInfo);

                return employee;
            }

            return cachedEmployee as EmployeeInfo;
        }

        public IList<EmployeeInfo> GetActiveByPaymentFrequency(int paymentFrequencyId)
        {
            return Find(e => e.Employee.IsActive /*&& e.PaymentFrequencyId == paymentFrequencyId*/).ToList();
        }

        public new IList<EmployeeInfo> GetAllActive() {
            return Find(e => e.Employee.IsActive).ToList();
        }

        public IList<EmployeeInfo> GetAllWithAllowance()
        {
            return Find(e => e.IsActive && e.Allowance > 0).ToList();
        }
    }
}
