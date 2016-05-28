using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Payroll.Models.Employee
{
    public class EmployeeLoanViewModel
    {
        public int EmployeeLoanId { get; set; }

        [DisplayName("Employee")]
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        [DisplayName("Loan")]
        public int LoanId { get; set; }
        public IEnumerable<SelectListItem> Loans { get; set; }

        [DisplayName("Payment Frequency")]
        public int FrequencyId { get; set; }
        public IEnumerable<SelectListItem> PaymentFrequencies { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid amount")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime LoanDate { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public DateTime PaymentStartDate { get; set; }


        //When to deduct
        public int WeeklyPaymentDayOfWeek { get; set; }
        public IEnumerable<SelectListItem> WeeklyPaymentDayOfWeekList { get; set; }

        public int BiMonthlyPaymentFirstDate { get; set; }

        public int BiMonthlyPaymentSecondDate { get; set; }

        public int MonthlyPaymentDate { get; set; }
    }
}