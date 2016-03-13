namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedRelationshipforEmployeeLoan : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.employee_loan", "EmployeeId");
            AddForeignKey("dbo.employee_loan", "EmployeeId", "dbo.employee", "EmployeeId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.employee_loan", "EmployeeId", "dbo.employee");
            DropIndex("dbo.employee_loan", new[] { "EmployeeId" });
        }
    }
}
