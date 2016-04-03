namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addfkemployeeWorkSchedule : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.employee_workschedule", "EmployeeId");
            AddForeignKey("dbo.employee_workschedule", "EmployeeId", "dbo.employee", "EmployeeId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.employee_workschedule", "EmployeeId", "dbo.employee");
            DropIndex("dbo.employee_workschedule", new[] { "EmployeeId" });
        }
    }
}
