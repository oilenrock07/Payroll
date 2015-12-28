using FluentNHibernate.Mapping;
using Payroll.Entities.Users;

namespace Payroll.Repository.Mappings
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Id(x => x.UserId);
            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.MiddleName);
            Map(x => x.Password);
            Map(x => x.UserName);
        }
    }
}
