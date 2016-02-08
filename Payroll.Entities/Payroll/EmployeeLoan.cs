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

        public int EmployeeId { get; set; }

        public int LoanId { get; set; }

        public int FrequencyId { get; set; }

        public decimal Amount { get; set; }

        public DateTime LoanDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        public DateTime PaymentStartDate { get; set; }

        public DateTime WeeklyPaymentDayOfWeek { get; set; }

        public DateTime BiMonthlyPaymentFirstDate { get; set; }

        public DateTime BiMonthlyPaymentSecondDate { get; set; }

        public DateTime MonthlyPaymentDate { get; set; }

    }
}
