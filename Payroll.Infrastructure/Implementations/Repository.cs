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

        protected DbSet<T> _dbSet
        {
            get { return _context.Set<T>(); }
        }

        public Repository(IDatabaseFactory databaseFactory)
        {
            _context = databaseFactory.GetContext();
            _sharedContext = true;
        }

        public T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet;
        }

        public IQueryable<T> Find(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return _dbSet.Where(expression);
        }

        public T Add(T entity)
        {
            _dbSet.Add(entity);

            if (!_sharedContext)
                _context.SaveChanges();

            return entity;
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);

            if (!_sharedContext)
                _context.SaveChanges();
        }

        public void Update(T entity, string[] propertyToUpdate)
        {
            _dbSet.Attach(entity);
            foreach (string property in propertyToUpdate)
                _context.Entry<T>(entity).Property(property).IsModified = true;

            if (!_sharedContext)
                _context.SaveChanges();
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);

            if (!_sharedContext)
                _context.SaveChanges();
        }

        public void ExecuteSqlCommand(string command, params object[] parameters)
        {
            _context.Database.ExecuteSqlCommand(command, parameters);
        }
    }
}
