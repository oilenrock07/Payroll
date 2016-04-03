using Payroll.Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("positions")]
    public class Position : BaseEntity
    {
        [Key]
        public int PositionId { get; set; }

        [StringLength(150)]
        public string PositionName { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }
    }
}
