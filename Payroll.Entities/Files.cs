using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("tbl_files")]
    public class Files
    {
        [Key]
        public int FileId { get; set; }

        [StringLength(250)]
        public string FileName { get; set; }

    }
}
