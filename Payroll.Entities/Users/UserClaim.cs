﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;

namespace Payroll.Entities.Users
{
    [Table("AspNetUserClaims")]
    public class UserClaim : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [StringLength(500)]
        public string ClaimType { get; set; }
        
        [StringLength(500)]
        public string ClaimValue { get; set; }

        [Column("User_Id")]
        [StringLength(250)]
        public string UserId { get; set; }
    }
}
