using System.Configuration;
using System.Data.Entity;
using Payroll.Entities.Payroll;
using Payroll.Entities.Users;

namespace Payroll.Entities.Contexts
{
    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
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
        public DbSet<AttendanceLog> AttendanceLog { get; set; }

        //Payroll
        public DbSet<Deduction> Deductions { get; set; }
        public DbSet<EmployeeAdjustment> EmployeeAdjustments { get; set; }
        public DbSet<EmployeeLeave> EmployeeLeaves { get; set; }
        public DbSet<EmployeeLoan> EmployeeLoans { get; set; }
        public DbSet<Payroll.Payroll> Payrolls { get; set; }

        //Users
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserClaim> UserClaims { get; set; }
        public DbSet<UserLogin> UserLogIns { get; set; }
        public DbSet<UserRole> UserRoles { get; set; } 

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
