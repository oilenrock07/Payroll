using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("holiday")]
    public class Holiday
    {
        [Key]
        public int HolidayId { get; set; }

        [StringLength(50)]
        public string HolidayName { get; set; }

        public bool IsRegularHoliday { get; set; }

        public DateTime Date { get; set; }

        public bool IsActive { get; set; }

        public int Year { get; set; }

    }
}
