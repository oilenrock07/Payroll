using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities.Users
{
    [Table("AspNetRoles")]
    public class Role
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
