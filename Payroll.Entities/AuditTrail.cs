using Payroll.Entities.Enums;

namespace Payroll.Entities
{
    public class AuditTrail
    {
        public int AuditTrailId { get; set; }

        //Logged in user
        public string Id { get; set; }

        public AuditTrailTransaction Transaction { get; set; }

        public AuditTrailTransactionType Type { get; set; }
    }
}
