using System.Configuration;
using System.Data.Entity;
using Payroll.Entities.Payroll;

namespace Payroll.Entities.Contexts
{
    public class PayrollContext : DbContext
    {
        public PayrollContext()
            : base(ConnectionString)
        {
            //Database.SetInitializer<PayrollContext>(null);
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DepartmentManager> DepartmentManagers { get; set; }
        public DbSet<EmployeeDepartment> EmployeeDepartments { get; set; }
        public DbSet<EmployeeFile> EmployeeFiles { get; set; }
        public DbSet<EmployeeInfo> EmployeeInfos { get; set; }
        public DbSet<EmployeeWorkSchedule> EmployeeWorkSchedules { get; set; }
        public DbSet<Files> Files { get; set; }
        public DbSet<Frequency> Frequencies { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanPayment> LoanPayments { get; set; }
        public DbSet<PaymentFrequency> PaymentFrequencies { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Tax> Taxes { get; set; }
        public DbSet<WorkSchedule> WorkSchedules { get; set; }

        //Payroll
        public DbSet<Deduction> Deductions { get; set; }
        public DbSet<EmployeeAdjustment> EmployeeAdjustments { get; set; }
        public DbSet<EmployeeLeave> EmployeeLeaves { get; set; }
        public DbSet<EmployeeLoan> EmployeeLoans { get; set; }
        public DbSet<Payroll.Payroll> Payrolls { get; set; }

        static string ConnectionString
        {

            get
            {
                string cs = "";
                switch (ConfigurationManager.AppSettings["DatabaseType"])
                {
                    case "MsSql":
                        cs = "ConnectionString.MsSql";
                        break;
                    case "MySql":
                        cs = "ConnectionString.MySql";
                        break;
                }

                return cs;
            }
        }
    }
}
