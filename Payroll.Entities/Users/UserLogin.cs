using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;

namespace Payroll.Entities.Users
{
    [Table("AspNetUserLogins")]
    public class UserLogin : BaseEntity
    {
        [Key]
        public string UserLoginId { get; set; }

        [StringLength(250)]
        public string UserId { get; set; }
        
        [StringLength(500)]
        public string LoginProvider { get; set; }
        
        [StringLength(500)]
        public string ProviderKey { get; set; }
    }
}
