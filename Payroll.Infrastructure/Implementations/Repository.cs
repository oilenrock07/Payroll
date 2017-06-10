using System;
using System.Linq;
using Payroll.Entities.Base;
using Payroll.Entities.Contexts;
using Payroll.Infrastructure.Interfaces;
using System.Data.Entity;
using System.Collections.Generic;

namespace Payroll.Infrastructure.Implementations
{
    public class Repository<T>  : IRepository<T>
        where T : BaseEntity
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

        public virtual IQueryable<T> GetAllActive()
        {
            return Find(e => e.IsActive);
        }

        public virtual IQueryable<T> Find(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return DbSet.Where(expression);
        }

        public virtual T Add(T entity)
        {
            entity.CreateDate = DateTime.Now;
           
            DbSet.Add(entity);

            if (!_sharedContext)
                _context.SaveChanges();

            return entity;
        }

        public virtual void Update(T entity)
        {
            DbSet.Attach(entity);
            entity.UpdateDate = DateTime.Now;
            if (!_sharedContext)
                _context.SaveChanges();
        }

        public virtual void Delete(T entity)
        {
            //DbSet.Remove(entity);
            DbSet.Attach(entity);

            entity.IsActive = false;

            if (!_sharedContext)
                _context.SaveChanges();
        }

        public virtual void DeleteAll(IList<T> entityList)
        {
            //DbSet.Remove(entity);
            foreach (T entity in entityList)
            {
                DbSet.Attach(entity);
                entity.IsActive = false;
            }

            if (!_sharedContext)
                _context.SaveChanges();
        }

        public virtual void PermanentDelete(T entity)
        {
            DbSet.Remove(entity);

            if (!_sharedContext)
                _context.SaveChanges();
        }

        public virtual void ExecuteSqlCommand(string command, params object[] parameters)
        {
            _context.Database.ExecuteSqlCommand(command, parameters);
        }

        public virtual void ExecuteSqlCommandTransaction(string command, params object[] parameters)
        {
            _context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, command, parameters);
        }

    }
}
