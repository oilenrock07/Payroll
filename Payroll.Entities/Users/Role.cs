using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities.Users
{
    [Table("AspNetRoles")]
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [StringLength(250)]
        public string Id { get; set; }

        [StringLength(250)]
        public string Name { get; set; }
    }
}
