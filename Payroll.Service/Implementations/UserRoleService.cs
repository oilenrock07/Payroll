using System.Collections.Generic;
using System.Linq;
using Payroll.Common.Extension;
using Payroll.Repository.Interface;
using Payroll.Repository.Models.User;
using Payroll.Service.Interfaces;

namespace Payroll.Service.Implementations
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;

        public UserRoleService(IUserRepository userRepository, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
        }

        public virtual IEnumerable<UserRoleDao> GetUsers()
        {
            var items = new List<UserRoleDao>();
            var users = _userRepository.GetAllActive().ToList();
            foreach (var user in users)
            {
                var userRole = user.MapItem<UserRoleDao>();

                var result = from usr in _userRoleRepository.GetAllActive()
                             join role in _roleRepository.GetAllActive() on usr.RoleId equals role.Id
                             where usr.UserId == user.Id
                             select role;

                userRole.Roles = result.ToList();
                items.Add(userRole);
            }

            return items;
        }
    }
}
