using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;
using Payroll.Entities.Enums;

namespace Payroll.Entities
{
    [Table("tax")]
    public class Tax : BaseEntity
    {
        [Key]
        public int TaxId { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        public FrequencyType Frequency { get; set; }

        public int NoOfDependents { get; set; }

        public decimal BaseAmount { get; set; }

        public decimal MaxAmount { get; set; }

        public decimal BaseTaxAmount { get; set; }

        public int OverPercentage { get; set; }

    }
}
