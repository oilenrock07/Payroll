
namespace Payroll.Repository.Models.Employee
{
    public class EmployeeNames
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName
        {
            get { return string.Format("{0}, {1}", FirstName, LastName); }
        }
    }
}
