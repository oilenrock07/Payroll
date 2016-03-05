using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Infrastructure.Implementations;

namespace Payroll.Repository.Repositories
{
    public class PaymentFrequencyRepository :  Repository<PaymentFrequency>, IPaymentFrequencyRepository
    {
        public PaymentFrequencyRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().PaymentFrequencies;
        }
    }
}
