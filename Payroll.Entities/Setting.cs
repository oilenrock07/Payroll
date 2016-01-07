using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("tbl_settings")]
    public class Setting
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public string Category { get; set; }
    }
}
