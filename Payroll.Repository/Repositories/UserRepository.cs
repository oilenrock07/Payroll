using System.Linq;
using Payroll.Entities.Users;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Infrastructure.Implementations;

namespace Payroll.Repository.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().Users;
        }

        public virtual User GetUserByUserNameAndPassword(string username, string hashedPassword)
        {
            return Find(x => x.UserName == username && x.PasswordHash == hashedPassword && x.IsActive).FirstOrDefault();
        }

        public virtual User GetById(string userId)
        {
            return Find(x => x.Id == userId).FirstOrDefault();
        }
    }
}
