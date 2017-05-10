using Payroll.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("company")]
    public class Company : BaseEntity
    {
        [Key]
        public int CompanyId { get; set; }
        [StringLength(100)]
        public string CompanyName { get; set; }

        [StringLength(20)]
        public string CompanyCode { get; set; }

        [StringLength(250)]
        public string Address { get; set; }

        [StringLength(250)]
        public string CompanyInfo { get; set; }
    }
}
