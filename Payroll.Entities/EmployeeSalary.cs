using Payroll.Entities.Base;
using Payroll.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Entities
{
    [Table("employee_salary")]
    public class EmployeeSalary : BaseEntity
    {
        [Key]
        public int EmploymentSalaryId { get; set; }

        public decimal Salary { get; set; }

        public FrequencyType SalaryFrequency { get; set; }

        public int EmployeeId { get; set;}
    }
}
