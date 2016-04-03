using System;

namespace Payroll.Entities.Base
{
    public abstract class BaseEntity
    {
        protected BaseEntity() {
            IsActive = true;
            CreateDate = new DateTime();
        }

        public bool IsActive { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ? UpdateDate { get; set; }
        
        //public User Owner { get; set; }
    }
}
