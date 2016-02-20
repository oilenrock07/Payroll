namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addedforeignkeytoemployeeandemployeeinfo : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.employee_info", "EmployeeId");
            AddForeignKey("dbo.employee_info", "EmployeeId", "dbo.employee", "EmployeeId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.employee_info", "EmployeeId", "dbo.employee");
            DropIndex("dbo.employee_info", new[] { "EmployeeId" });
        }
    }
}
