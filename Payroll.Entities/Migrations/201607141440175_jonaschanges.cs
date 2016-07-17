namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class jonaschanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.employee_adjustment", "PayrollId", c => c.Int());
            CreateIndex("dbo.employee_payroll_item", "EmployeeId");
            AddForeignKey("dbo.employee_payroll_item", "EmployeeId", "dbo.employee", "EmployeeId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.employee_payroll_item", "EmployeeId", "dbo.employee");
            DropIndex("dbo.employee_payroll_item", new[] { "EmployeeId" });
            DropColumn("dbo.employee_adjustment", "PayrollId");
        }
    }
}
