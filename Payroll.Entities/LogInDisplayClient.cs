using Payroll.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Payroll.Entities
{
    public class LogInDisplayClient : BaseEntity
    {
        [Key]
        public int LogInDisplayClientId { get; set; }

        public string IpAddress { get; set; }

        public string ClientId { get; set; }
    }
}
