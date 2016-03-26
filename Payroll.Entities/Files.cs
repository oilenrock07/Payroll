using Payroll.Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("files")]
    public class Files : BaseEntity
    {
        [Key]
        public int FileId { get; set; }

        [StringLength(250)]
        public string FileName { get; set; }

    }
}
