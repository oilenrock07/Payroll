namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class payrollupdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.employee", "EmployeeCode", c => c.String(nullable: false, maxLength: 250, storeType: "nvarchar"));
            CreateIndex("dbo.payroll", "EmployeeId");
            AddForeignKey("dbo.payroll", "EmployeeId", "dbo.employee", "EmployeeId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.payroll", "EmployeeId", "dbo.employee");
            DropIndex("dbo.payroll", new[] { "EmployeeId" });
            AlterColumn("dbo.employee", "EmployeeCode", c => c.String(maxLength: 250, storeType: "nvarchar"));
        }
    }
}
