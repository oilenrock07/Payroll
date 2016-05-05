using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;

namespace Payroll.Entities
{
    [Table("employee_machine")]
    public class EmployeeMachine : BaseEntity
    {
        [Key]
        public int EmployeeMachineId { get; set; }

        public int EmployeeId { get; set; }

        [ForeignKey("Machine")]
        public int MachineId { get; set; }
        public virtual Machine Machine { get; set; }
    }
}
