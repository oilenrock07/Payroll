using System.Collections.Generic;
using Payroll.Repository.Models.User;

namespace Payroll.Service.Interfaces
{
    public interface IUserRoleService
    {
        IEnumerable<UserRoleDao> GetUsers();
    }
}
