using System;
using System.Linq;
using Payroll.Entities.Contexts;
using Payroll.Infrastructure.Interfaces;
using System.Data.Entity;

namespace Payroll.Infrastructure.Implementations
{
    public class Repository<T>  : IRepository<T>
        where T : class
    {

        protected readonly bool _sharedContext = false;
        protected readonly PayrollContext _context;

        protected virtual DbSet<T> _dbSet
        {
            get { return _context.Set<T>(); }
        }

        public Repository(IDatabaseFactory databaseFactory)
        {
            _context = databaseFactory.GetContext();
            _sharedContext = true;
        }

        public virtual T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public virtual IQueryable<T> GetAll()
        {
            return _dbSet;
        }

        public virtual IQueryable<T> Find(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return _dbSet.Where(expression);
        }

        public virtual T Add(T entity)
        {
            _dbSet.Add(entity);

            if (!_sharedContext)
                _context.SaveChanges();

            return entity;
        }

        public virtual void Update(T entity)
        {
            _dbSet.Attach(entity);

            if (!_sharedContext)
                _context.SaveChanges();
        }

        public virtual void Update(T entity, string[] propertyToUpdate)
        {
            _dbSet.Attach(entity);
            foreach (string property in propertyToUpdate)
                _context.Entry<T>(entity).Property(property).IsModified = true;

            if (!_sharedContext)
                _context.SaveChanges();
        }

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);

            if (!_sharedContext)
                _context.SaveChanges();
        }

        public virtual void ExecuteSqlCommand(string command, params object[] parameters)
        {
            _context.Database.ExecuteSqlCommand(command, parameters);
        }
    }
}
