using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities.Users
{
    [Table("AspNetUserRoles")]
    public class UserRole
    {
        [Key]
        public int UserRoleId { get; set; }

        [StringLength(250)]
        public string UserId { get; set; }
        
        
        [StringLength(250)]
        public string RoleId { get; set; }
    }
}
