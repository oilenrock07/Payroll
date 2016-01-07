using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("tbl_employee")]
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
            
        [StringLength(250)]
        public string EmployeeCode { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string MiddleName { get; set; }
        
        public DateTime BirthDate { get; set; }
    }
}
