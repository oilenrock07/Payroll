using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;

namespace Payroll.Entities
{
    [Table("machines")]
    public class Machine : BaseEntity
    {
        [Key]
        public int MachineId { get; set; }

        public string IpAddress { get; set; }
    }
}
