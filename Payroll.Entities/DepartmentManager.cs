using Payroll.Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("department_manager")]
    public class DepartmentManager : BaseEntity
    {
        [Key]
        public int DepartmentManagerId { get; set; }

        public int DepartmentId { get; set; }

        public int ManagerId { get; set; }
    }
}
