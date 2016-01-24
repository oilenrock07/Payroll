using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities.Users
{
    [Table("AspNetUserRoles")]
    public class UserRole
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }
}
