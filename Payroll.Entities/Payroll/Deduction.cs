using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities.Payroll
{
    [Table("deduction")]
    public class Deduction
    {
        [Key]
        public int DeductionId { get; set; }

        [StringLength(50)]
        public string DeductionName { get; set; }

        public string Remarks { get; set; }
    }
}
