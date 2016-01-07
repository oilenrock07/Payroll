using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("tbl_employee_department")]
    public class EmployeeDepartment
    {
        [Key]
        public int EmployeeDepartmentId { get; set; }

        public int EmployeeId { get; set; }

        public int DepartmentId { get; set; }
    }
}
