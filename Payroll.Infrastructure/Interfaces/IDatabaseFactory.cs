using Payroll.Entities.Contexts;

namespace Payroll.Infrastructure.Interfaces
{
    public interface IDatabaseFactory
    {
        PayrollContext GetContext();
    }
}
