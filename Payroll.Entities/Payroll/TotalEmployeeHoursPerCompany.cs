using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Payroll.Entities.Payroll
{
    [Table("employee_hours_total_per_company")]
    public class TotalEmployeeHoursPerCompany : TotalEmployeeHours
    {
        [Key]
        public int TotalEmployeeHoursPerCompanyId { get; set; }

        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}
