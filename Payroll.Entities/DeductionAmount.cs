using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;

namespace Payroll.Entities
{
    [Table("deduction_amount")]
    public class DeductionAmount : BaseEntity
    {
        [Key]
        public int DeductionAmountId { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        public int DeductionId { get; set; }

        public int Frequency { get; set; }

        public decimal MinBaseAmount { get; set; }

        public decimal MaxBaseAmount { get; set; }

        public decimal Value { get; set; }

        public bool IsPercentage { get; set; }
    }
}
