using Payroll.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities.Payroll
{
    [Table("employee_payroll_item_per_company")]
    public class EmployeePayrollItemPerCompany : EmployeePayrollItem
    {
        [Key]
        public int EmployeePayrollItemPerCompanyId { get; set; }

        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}
