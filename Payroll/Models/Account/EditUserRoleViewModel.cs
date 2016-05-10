using System.Collections.Generic;
using Payroll.Repository.Models.User;
using Payroll.Entities.Users;
using System.ComponentModel.DataAnnotations;

namespace Payroll.Models.Account
{
    public class EditUserRoleViewModel
    {
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string CheckedRoles { get; set; }

        public UserRoleDao UserRole { get; set; }
        public IEnumerable<Role> Roles { get; set; }
    }
}