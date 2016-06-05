using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;

namespace Payroll.Entities.Payroll
{
    [Table("deduction")]
    public class Deduction : BaseEntity
    {
        public Deduction() : base()
        {
            IsCustomizable = true;
        }

        [Key]
        public int DeductionId { get; set; }

        [StringLength(50)]
        public string DeductionName { get; set; }

        [StringLength(2500)]
        public string Remarks { get; set; }

        public bool IsCustomizable { get; set; }
    }
}
