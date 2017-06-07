using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Payroll.Base;

namespace Payroll.Entities.Payroll
{
    [Table("employee_payroll_item_per_company")]
    public class EmployeePayrollItemPerCompany : PayrollItemBase
    {
        [Key]
        public int EmployeePayrollItemPerCompanyId { get; set; }

        public int PayrollPerCompanyId { get; set; }

        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}
