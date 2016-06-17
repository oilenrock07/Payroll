
namespace Payroll.Models.Employee
{
    public class EmployeeDeductionViewModel
    {
        public int DeductionId { get; set; }
        public string DeductionName { get; set; }
        public decimal Amount { get; set; }
        public bool IsChecked { get; set; }
    }
}