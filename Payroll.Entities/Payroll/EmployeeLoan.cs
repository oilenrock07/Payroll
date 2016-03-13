using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities.Payroll
{
    [Table("employee_loan")]
    public class EmployeeLoan
    {
        [Key]
        public int EmployeeLoanId { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        [ForeignKey("Loan")]
        public int LoanId { get; set; }
        public virtual Loan Loan { get; set; }

        public int FrequencyId { get; set; }

        public decimal Amount { get; set; }

        public DateTime LoanDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        public DateTime PaymentStartDate { get; set; }


        //When to deduct
        public int WeeklyPaymentDayOfWeek { get; set; }

        public int BiMonthlyPaymentFirstDate { get; set; }

        public int BiMonthlyPaymentSecondDate { get; set; }

        public int MonthlyPaymentDate { get; set; }

    }
}
