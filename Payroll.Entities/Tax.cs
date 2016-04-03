using Payroll.Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("tax")]
    public class Tax : BaseEntity
    {
        [Key]
        public int TaxId { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        public int Frequency { get; set; }

        public int NoOfDependents { get; set; }

        public decimal BaseAmount { get; set; }

        public int OverPercentage { get; set; }
    }
}
