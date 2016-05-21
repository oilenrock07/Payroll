namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deductionchange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.deduction", "IsCustomizable", c => c.Boolean(nullable: false));
            AddColumn("dbo.employee_payroll_deduction", "EmployeePayrollId", c => c.Int(nullable: false));
            AddColumn("dbo.employee_payroll_deduction", "DeductionDate", c => c.DateTime(nullable: false, precision: 0));
            DropColumn("dbo.employee_payroll_deduction", "PayrollDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.employee_payroll_deduction", "PayrollDate", c => c.DateTime(nullable: false, precision: 0));
            DropColumn("dbo.employee_payroll_deduction", "DeductionDate");
            DropColumn("dbo.employee_payroll_deduction", "EmployeePayrollId");
            DropColumn("dbo.deduction", "IsCustomizable");
        }
    }
}
