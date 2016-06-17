namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFKToEmployeeDeduction : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.employee_deduction", "DeductionId");
            AddForeignKey("dbo.employee_deduction", "DeductionId", "dbo.deduction", "DeductionId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.employee_deduction", "DeductionId", "dbo.deduction");
            DropIndex("dbo.employee_deduction", new[] { "DeductionId" });
        }
    }
}
