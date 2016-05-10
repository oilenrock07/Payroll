using Payroll.Entities.Users;
using Payroll.Infrastructure.Interfaces;

namespace Payroll.Repository.Interface
{
    public interface IUserRepository : IRepository<User>
    {
        User GetUserByUserNameAndPassword(string username, string hashedPassword);
        User GetById(string userId);
    }
}
