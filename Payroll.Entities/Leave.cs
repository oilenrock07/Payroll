using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("tbl_leave")]
    public class Leave
    {
        [Key]
        public int LeaveId { get; set; }

        [StringLength(250)]
        public string LeaveName { get; set; }

        public bool IsActive { get; set; }

        public bool IsRefundable { get; set; }

        public int Count { get; set; }

        public string Description { get; set; }

    }
}
