using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;

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
