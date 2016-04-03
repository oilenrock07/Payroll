namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addfkemployeeWorkHours : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.employee_hours", "EmployeeId");
            AddForeignKey("dbo.employee_hours", "EmployeeId", "dbo.employee", "EmployeeId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.employee_hours", "EmployeeId", "dbo.employee");
            DropIndex("dbo.employee_hours", new[] { "EmployeeId" });
        }
    }
}
