using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("employee_machine")]
    public class EmployeeMachine
    {
        [Key]
        public int EmployeeMachineId { get; set; }

        public int EmployeeId { get; set; }

        public int MachineId { get; set; }
    }
}
