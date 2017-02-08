using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;

namespace Payroll.Entities
{
    [Table("holiday")]
    public class Holiday : BaseEntity
    {
        [Key]
        public int HolidayId { get; set; }

        [StringLength(50)]
        [Required]
        public string HolidayName { get; set; }

        public bool IsRegularHoliday { get; set; }

        public DateTime Date { get; set; }
        
        public int Year { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public bool IsAlwaysPayable { get; set; }
    }
}
