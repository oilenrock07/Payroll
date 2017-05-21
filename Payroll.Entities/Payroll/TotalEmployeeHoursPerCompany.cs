using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;

namespace Payroll.Entities.Payroll
{
    [Table("employee_hours_total_per_company")]
    public class TotalEmployeeHoursPerCompany : BaseEntity
    {
        [Key]
        public int TotalEmployeeHoursPerCompanyId { get; set; }

        [ForeignKey("TotalEmployeeHours")]
        public int TotalEmployeeHoursId { get; set; }
        public virtual TotalEmployeeHours TotalEmployeeHours { get; set; }

        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public double Hours { get; set; }
    }
}
