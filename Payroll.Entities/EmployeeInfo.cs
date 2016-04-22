using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;

namespace Payroll.Entities
{
    [Table("employee_info")]
    public class EmployeeInfo : BaseEntity
    {
        [Key]
        public int EmploymentInfoId { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        [ForeignKey("PaymentFrequency")]
        public int ? PaymentFrequencyId { get; set; }
        public virtual PaymentFrequency PaymentFrequency{ get; set; }

        public int ? PositionId { get; set; }

        [ForeignKey("EmployeeSalary")]
        public int ? EmployeeSalaryId { get; set; }
        public virtual EmployeeSalary EmployeeSalary { get; set; }

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
