using Payroll.Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("loan")]
    public class Loan : BaseEntity
    {
        [Key]
        public int LoanId { get; set; }

        [StringLength(50)]
        public string LoanName { get; set; }

        public decimal Min { get; set; }

        public decimal Max { get; set; }

        public decimal Interest { get; set; }

        public int LoanPeriod { get; set; }

        public int LoanPeriodNumber { get; set; }

    }
}
