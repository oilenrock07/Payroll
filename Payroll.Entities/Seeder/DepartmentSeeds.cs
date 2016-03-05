using System.Collections.Generic;

namespace Payroll.Entities.Seeder
{
    public class DepartmentSeeds : ISeeders<Department>
    {
        public IEnumerable<Department> GetDefaultSeeds()
        {
            return new List<Department>
            {
                new Department {DepartmentName = "FFM"},
                new Department {DepartmentName = "DEXA"},
                new Department {DepartmentName = "ATHENA"},
                new Department {DepartmentName = "PRODUCTION"},
                new Department {DepartmentName = "SEC. & ENGR."},
                new Department {DepartmentName = "TANNERY"},
            };
        }
    }
}
