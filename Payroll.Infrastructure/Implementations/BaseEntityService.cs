using Payroll.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Payroll.Infrastructure.Entities;

namespace Payroll.Infrastructure.Implementations
{
    public class BaseEntityService<T> : IBaseEntityService<T>
        where T : BaseEntity
    {
        private readonly IRepository<T> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public BaseEntityService(IRepository<T> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public T Add(T entity)
        {
            entity.IsActive = true;
            entity.CreateDate = new DateTime();

            return _repository.Add(entity);
        }

        public void Delete(T entity)
        {
            _repository.Update(entity);
            entity.IsActive = false;
        }

        public void ExecuteSqlCommand(string command, params object[] parameters)
        {
            _repository.ExecuteSqlCommand(command, parameters);
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> expression)
        {
            return _repository.Find(expression);
        }

        public IQueryable<T> GetAll()
        {
            return _repository.GetAll();
        }

        public T GetById(int id)
        {
            return _repository.GetById(id);
        }

        public void Update(T entity)
        {
            _repository.Update(entity);

            entity.UpdateDate = new DateTime();
        }
    }
}
