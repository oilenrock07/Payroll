namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removefkworkschedule : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("employee_workschedule", "EmployeeId", "employee");
            DropIndex("employee_workschedule", new[] { "EmployeeId" });
        }
        
        public override void Down()
        {
            CreateIndex("employee_workschedule", "EmployeeId");
            AddForeignKey("employee_workschedule", "EmployeeId", "employee", "EmployeeId", cascadeDelete: true);
        }
    }
}
