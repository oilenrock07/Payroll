
using System.ComponentModel.DataAnnotations;
using Payroll.Entities.Base;

namespace Payroll.Entities
{
    public class Adjustment : BaseEntity
    {
        public int AdjustmentId { get; set; }

        [Required]
        public string AdjustmentName { get; set; }

        public string Description { get; set; }
    }
}
