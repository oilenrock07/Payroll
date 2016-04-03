using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;

namespace Payroll.Entities
{
    [Table("loan_payment")]
    public class LoanPayment : BaseEntity
    {
        [Key]
        public int LoanPaymentId { get; set; }

        public int EmployeeLoanId { get; set; }

        public DateTime PaymentDate { get; set; }

        public decimal Amount { get; set; }
    }
}
