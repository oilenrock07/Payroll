using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("employee_info")]
    public class EmployeeInfo
    {
        [Key]
        public int EmploymentInfoId { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        public int ? PaymentFrequencyId { get; set; }

        public int ? PositionId { get; set; }

        public decimal Salary { get; set; }

        public decimal ? Allowance { get; set; }

        [StringLength(50)]
        public string TIN { get; set; }

        [StringLength(50)]
        public string SSS { get; set; }


        [StringLength(50)]
        public string GSIS { get; set; }

        [StringLength(50)]
        public string PAGIBIG { get; set; }

        [StringLength(50)]
        public string PhilHealth { get; set; }

        public int Dependents { get; set; }

        public bool Married { get; set; }

    }
}
