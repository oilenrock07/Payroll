
using System.Collections.Generic;
namespace Payroll.Entities.Seeder
{
    public interface ISeeders<out T> where T : class
    {
        IEnumerable<T> GetDefaultSeeds();
    }
}
