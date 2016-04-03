using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;

namespace Payroll.Entities
{
    [Table("settings")]
    public class Setting : BaseEntity
    {
        [Key]
        public int SettingId { get; set; }

        [StringLength(250)]
        public string SettingKey { get; set; }

        [StringLength(5000)]
        public string Value { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(250)]
        public string Category { get; set; }
    }
}
