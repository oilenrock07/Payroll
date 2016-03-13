using System;

namespace Payroll.Repository.Models
{
    public class EmployeeLoanDao
    {
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

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string LoanName { get; set; }
    }
}
