using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Payroll.Entities.Base;

namespace Payroll.Entities
{
    [Table("employee")]
    public class Employee : BaseEntity
    {

        [Key]
        public int EmployeeId { get; set; }
            
        [StringLength(250)]
        [Required]
        public string EmployeeCode { get; set; }

        [StringLength(50)]
        [Required]
        public string FirstName { get; set; }

        [StringLength(50)]
        [Required]
        public string LastName { get; set; }

        [StringLength(50)]
        public string MiddleName { get; set; }

        [StringLength(100)]
        public string NickName { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [StringLength(500)]
        public string Picture { get; set; }

        public int Gender { get; set; }

        //For clock in/out (biomertrics and rfid)
        public int Privilege { get; set; }

        [StringLength(500)]
        [NotMapped]
        public string FullName
        {
            get { return String.Format("{0} {1}", FirstName, LastName); }
        }
    }
}
