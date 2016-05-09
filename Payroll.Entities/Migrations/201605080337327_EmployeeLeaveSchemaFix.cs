namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmployeeLeaveSchemaFix : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.employee_leave", "LeaveStatus", c => c.Int(nullable: false));
            CreateIndex("dbo.employee_leave", "EmployeeId");
            AddForeignKey("dbo.employee_leave", "EmployeeId", "dbo.employee", "EmployeeId", cascadeDelete: true);
            DropColumn("dbo.employee_leave", "IsApproved");
        }
        
        public override void Down()
        {
            AddColumn("dbo.employee_leave", "IsApproved", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.employee_leave", "EmployeeId", "dbo.employee");
            DropIndex("dbo.employee_leave", new[] { "EmployeeId" });
            DropColumn("dbo.employee_leave", "LeaveStatus");
        }
    }
}
