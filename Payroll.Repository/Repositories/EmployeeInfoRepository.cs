using System.Linq;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Interface;
using System.Collections.Generic;

namespace Payroll.Repository.Repositories
{
    public class EmployeeInfoRepository : Repository<EmployeeInfo>, IEmployeeInfoRepository
    {
        public EmployeeInfoRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().EmployeeInfos;
        }

        public EmployeeInfo GetByEmployeeId(int employeeId)
        {
            return Find(x => x.Employee.EmployeeId == employeeId).FirstOrDefault();
        }

        public IList<EmployeeInfo> GetActiveByPaymentFrequency(int paymentFrequencyId)
        {
            return Find(e => e.Employee.IsActive && e.PaymentFrequencyId == paymentFrequencyId).ToList();
        }
    }
}
