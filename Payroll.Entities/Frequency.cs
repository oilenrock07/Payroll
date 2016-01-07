using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("tbl_frequency")]
    public class Frequency
    {
        [Key]
        public int FrequencyId { get; set; }

        [StringLength(50)]
        public string FrequencyName { get; set; }
    }
}
