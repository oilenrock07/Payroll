using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities.Users
{
    [Table("AspNetUserClaims")]
    public class UserClaim
    {
        [Key]
        public int Id { get; set; }

        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        [Column("User_Id")]
        public string UserId { get; set; }
    }
}
