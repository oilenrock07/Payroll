using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities.Users
{
    [Table("AspNetUserRoles")]
    public class UserRole
    {
        [Key]
        public int UserRoleId { get; set; }
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }
}
