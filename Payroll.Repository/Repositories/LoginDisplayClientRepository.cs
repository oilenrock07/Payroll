using Payroll.Entities;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;

namespace Payroll.Repository.Repositories
{
    public class LoginDisplayClientRepository : Repository<LogInDisplayClient>, ILoginDisplayClientRepository
    {
        public LoginDisplayClientRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }
    }
}
