namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addfkattendance : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.attendance", "EmployeeId");
            AddForeignKey("dbo.attendance", "EmployeeId", "dbo.employee", "EmployeeId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.attendance", "EmployeeId", "dbo.employee");
            DropIndex("dbo.attendance", new[] { "EmployeeId" });
        }
    }
}
