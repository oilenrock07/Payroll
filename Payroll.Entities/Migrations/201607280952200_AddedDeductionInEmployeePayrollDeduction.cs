namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDeductionInEmployeePayrollDeduction : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.employee_payroll_deduction", "DeductionId");
            AddForeignKey("dbo.employee_payroll_deduction", "DeductionId", "dbo.deduction", "DeductionId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.employee_payroll_deduction", "DeductionId", "dbo.deduction");
            DropIndex("dbo.employee_payroll_deduction", new[] { "DeductionId" });
        }
    }
}
