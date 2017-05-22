using System.Linq;
using Payroll.Entities;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Constants;
using Payroll.Repository.Interface;
using CacheManager.Core;

namespace Payroll.Repository.Repositories
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ICacheManager<object> _cacheManager;
        private const string COMPANY_CACHE_KEY = "AllCompanies";

        public CompanyRepository(IDatabaseFactory databaseFactory, ICacheManager<object> cacheManager = null)
            : base(databaseFactory)
        {
            _cacheManager = cacheManager;
        }

        public override IQueryable<Company> GetAllActive()
        {
            var cachedCompanies = _cacheManager != null
                ? _cacheManager.Get(COMPANY_CACHE_KEY, CacheRegion.Companies)
                : null;

            if (cachedCompanies == null)
            {
                var companies = base.GetAllActive();
                if (_cacheManager != null)
                {
                    _cacheManager.Add(COMPANY_CACHE_KEY, companies, CacheRegion.Companies);
                }

                return companies;
            }

            return cachedCompanies as IQueryable<Company>;
        }


        public override void Update(Company entity)
        {
            base.Update(entity);

            var cachedCompany = _cacheManager != null ? _cacheManager.Get(COMPANY_CACHE_KEY, CacheRegion.Companies) : null;
            if (cachedCompany != null)
                _cacheManager.Remove(COMPANY_CACHE_KEY, CacheRegion.Companies);
        }

        public override Company Add(Company entity)
        {
            var addedEntity = base.Add(entity);

            var cachedCompany = _cacheManager != null ? _cacheManager.Get(COMPANY_CACHE_KEY, CacheRegion.Companies) : null;
            if (cachedCompany != null)
                _cacheManager.Remove(COMPANY_CACHE_KEY, CacheRegion.Companies);

            return addedEntity;
        }
    }
}
