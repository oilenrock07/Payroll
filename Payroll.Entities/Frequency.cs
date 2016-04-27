using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;
using Payroll.Entities.Enums;

namespace Payroll.Entities
{
    [Table("frequency")]
    public class Frequency : BaseEntity
    {
        [Key]
        public int FrequencyId { get; set; }

        [StringLength(50)]
        public string FrequencyName { get; set; }

        public FrequencyType? FrequencyType { get; set; }
    }
}
