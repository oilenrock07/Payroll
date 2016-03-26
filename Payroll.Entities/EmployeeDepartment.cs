using Payroll.Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("employee_department")]
    public class EmployeeDepartment : BaseEntity
    {
        [Key]
        public int EmployeeDepartmentId { get; set; }

        public int EmployeeId { get; set; }

        public int DepartmentId { get; set; }
    }
}
