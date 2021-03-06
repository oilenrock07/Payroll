﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;

namespace Payroll.Entities.Users
{
    [Table("AspNetRoles")]
    public class Role : BaseEntity
    {
        [Key]
        public int RoleId { get; set; }

        [StringLength(250)]
        public string Id { get; set; }

        [StringLength(250)]
        public string Name { get; set; }
    }
}
