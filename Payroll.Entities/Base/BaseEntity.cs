using Payroll.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Infrastructure.Entities
{
    public abstract class BaseEntity
    {
        public BaseEntity(){
            IsActive = true;
            CreateDate = new DateTime();
        }

        public Boolean IsActive { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ? UpdateDate { get; set; }
        
        public User Owner { get; set; }
    }
}
