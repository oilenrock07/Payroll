using System;
using System.Linq;
using System.Linq.Expressions;

namespace Payroll.Infrastructure.Interfaces
{
    public interface IRepository<T> where T : class 
    {
        T GetById(int id);

        IQueryable<T> GetAll();

        IQueryable<T> GetAllActive();

        IQueryable<T> Find(Expression<Func<T, bool>> expression);
        
        T Add(T entity);

        void Update(T entity);

        void Delete(T entity);

        void ExecuteSqlCommand(string command, params object[] parameters);
    }
}
