using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Payroll.Base;


namespace Payroll.Entities.Payroll
{
    [Table("employee_payroll_per_company")]
    public class EmployeePayrollPerCompany : PayrollBase
    {
        [Key]
        public int EmployeePayrollPerCompanyId { get; set; }

        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}
