using System.Configuration;
using Payroll.Entities.Contexts;
using Payroll.Infrastructure.Interfaces;

namespace Payroll.Infrastructure.Implementations
{
    public class DatabaseFactory : IDatabaseFactory
    {
        private PayrollContext _context;

        public DatabaseFactory()
        {
        }

        public DatabaseFactory(PayrollContext context)
        {
            _context = context;
        }

        public virtual PayrollContext GetContext()
        {
            if (_context != null) return _context;

            _context = new PayrollContext();
            return _context;
        }
    }
}
