using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("positions")]
    public class Position
    {
        [Key]
        public int PositionId { get; set; }

        [StringLength(150)]
        public string PositionName { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }
    }
}
