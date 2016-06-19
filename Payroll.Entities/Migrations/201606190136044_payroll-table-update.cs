namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class payrolltableupdate : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.payroll", "EmployeeId");
            AddForeignKey("dbo.payroll", "EmployeeId", "dbo.employee", "EmployeeId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.payroll", "EmployeeId", "dbo.employee");
            DropIndex("dbo.payroll", new[] { "EmployeeId" });
        }
    }
}
