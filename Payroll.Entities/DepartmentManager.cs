using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("tbl_department_manager")]
    public class DepartmentManager
    {
        [Key]
        public int DepartmentManagerId { get; set; }

        public int DepartmentId { get; set; }

        public int ManagerId { get; set; }
    }
}
