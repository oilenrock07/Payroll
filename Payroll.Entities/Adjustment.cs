
using System.ComponentModel.DataAnnotations;
using Payroll.Entities.Base;
using Payroll.Entities.Enums;

namespace Payroll.Entities
{
    public class Adjustment : BaseEntity
    {
        public int AdjustmentId { get; set; }

        [Required]
        public string AdjustmentName { get; set; }

        public string Description { get; set; }

        public AdjustmentType AdjustmentType { get; set; }
    }
}
