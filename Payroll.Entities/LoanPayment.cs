using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("tbl_loan_payment")]
    public class LoanPayment
    {
        [Key]
        public int LoanPaymentId { get; set; }

        public int EmployeeLoanId { get; set; }

        public DateTime PaymentDate { get; set; }

        public decimal Amount { get; set; }
    }
}
