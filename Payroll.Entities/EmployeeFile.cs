using Payroll.Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("employee_files")]
    public class EmployeeFile : BaseEntity
    {
        [Key]
        public int EmployeeFileId { get; set; }

        public int EmployeeId { get; set; }

        public int FileId { get; set; }
    }
}
