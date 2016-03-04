using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("payment_frequency")]
    public class PaymentFrequency
    {
        [Key]
        public int PaymentFrequencyId { get; set; }

        [ForeignKey("Frequency")]
        public int FrequencyId { get; set; }
        public virtual Frequency Frequency { get; set; }


        public bool IsActive { get; set; }

        public int ? WeeklyStartDayOfWeek { get; set; }

        public int ? MonthlyStartDay { get; set; }

        public int ? MonthlyEndDay { get; set; }
    }
}
