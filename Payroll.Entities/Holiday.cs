using Payroll.Infrastructure.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    }
}
