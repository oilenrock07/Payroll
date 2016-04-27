using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities.Users
{
    [Table("AspNetUsers")]
    public class User
    {
        [Key]
        public string Id { get; set; }

        [StringLength(250)]
        public string UserName { get; set; }
        
        [StringLength(500)]
        public string PasswordHash { get; set; }
        
        [StringLength(500)]
        public string SecurityStamp { get; set; }
        
        [StringLength(500)]
        public string Discriminator { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
