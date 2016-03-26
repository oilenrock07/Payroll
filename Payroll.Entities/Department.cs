using Payroll.Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("department")]
    public class Department : BaseEntity
    {
        public int DepartmentId { get; set; }

        [StringLength(250)]
        [Required]
        public string DepartmentName { get; set; }
    }
}
