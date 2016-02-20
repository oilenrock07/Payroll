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

        private IDbSet<T> _dbset;
        public virtual IDbSet<T> DbSet
        {
            get
            {
                return _dbset ?? _context.Set<T>();
            }
            set { _dbset = value; } 
        }
	

        public Repository(IDatabaseFactory databaseFactory)
        {
            _context = databaseFactory.GetContext();
            _sharedContext = true;
        }

        public virtual T GetById(int id)
        {
            return DbSet.Find(id);
        }

        public virtual IQueryable<T> GetAll()
        {
            return DbSet;
        }

        public virtual IQueryable<T> Find(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return DbSet.Where(expression);
        }

        public virtual T Add(T entity)
        {
            DbSet.Add(entity);

            if (!_sharedContext)
                _context.SaveChanges();

            return entity;
        }

        public virtual void Update(T entity)
        {
            DbSet.Attach(entity);
            
            if (!_sharedContext)
                _context.SaveChanges();
        }

        public virtual void Delete(T entity)
        {
            DbSet.Remove(entity);

            if (!_sharedContext)
                _context.SaveChanges();
        }

        public virtual void ExecuteSqlCommand(string command, params object[] parameters)
        {
            _context.Database.ExecuteSqlCommand(command, parameters);
        }
    }
}
