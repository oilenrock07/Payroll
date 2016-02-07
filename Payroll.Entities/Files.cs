using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("files")]
    public class Files
    {
        [Key]
        public int FileId { get; set; }

        [StringLength(250)]
        public string FileName { get; set; }

    }
}
