using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Entities.Payroll
{
    [Table("employee_deduction")]
    public class EmployeeDeduction
    {
        [Key]
        public int EmployeeDeductionId { get; set; }

        public int DeductionId { get; set; }

        public decimal Amount { get; set; }

        public int EmployeeId { get; set; }
    }
}
