using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities.Users
{
    [Table("AspNetRoles")]
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
