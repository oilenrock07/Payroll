using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("logs")]
    public class Log
    {
        [Key]
        public int LogId { get; set; }
        public DateTime DateLogged { get; set; }
        
        [StringLength(15)]
        public string Level { get; set; }

        public string UserName { get; set; }

        public string Message { get; set; }
        public string Url { get; set; }

        public string Logger { get; set; }
        public string Callsite { get; set; }
        public string Exception { get; set; }
    }
}
