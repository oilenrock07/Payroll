using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;
using Payroll.Entities.Enums;

namespace Payroll.Entities
{
    [Table("employee_info_history")]
    public class EmployeeInfoHistory : BaseEntity
    {
        public EmployeeInfoHistory() : base()
        {
            Dependents = 0;
        }

        [Key]
        public int EmployeeInfoHistoryId { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        
        public int ? PositionId { get; set; }

        public decimal Salary { get; set; }

        public FrequencyType SalaryFrequency { get; set; }

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

        public DateTime DateHired { get; set; }

        public int EmploymentStatus { get; set; }

        public DateTime? CustomDate1 { get; set; }
        public DateTime? CustomDate2 { get; set; }

        [StringLength(250)]
        public string CustomString1 { get; set; }
        [StringLength(250)]
        public string CustomString2 { get; set; }

        public decimal CustomDecimal1 { get; set; }
        public decimal CustomDecimal2 { get; set; }

        
    }
}
