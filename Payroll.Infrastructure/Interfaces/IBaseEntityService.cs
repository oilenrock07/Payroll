using Payroll.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Infrastructure.Interfaces
{
    public interface IBaseEntityService<T> where T : BaseEntity
    {
        T GetById(int id);
        IQueryable<T> GetAll();
        IQueryable<T> Find(Expression<Func<T, bool>> expression);

        T Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void ExecuteSqlCommand(string command, params object[] parameters);
    }
}
