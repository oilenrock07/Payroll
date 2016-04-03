using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;

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
