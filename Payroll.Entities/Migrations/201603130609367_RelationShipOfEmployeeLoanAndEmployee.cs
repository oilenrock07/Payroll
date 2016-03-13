namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RelationShipOfEmployeeLoanAndEmployee : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.employee_loan", "WeeklyPaymentDayOfWeek", c => c.Int(nullable: false));
            AlterColumn("dbo.employee_loan", "BiMonthlyPaymentFirstDate", c => c.Int(nullable: false));
            AlterColumn("dbo.employee_loan", "BiMonthlyPaymentSecondDate", c => c.Int(nullable: false));
            AlterColumn("dbo.employee_loan", "MonthlyPaymentDate", c => c.Int(nullable: false));
            CreateIndex("dbo.employee_loan", "LoanId");
            AddForeignKey("dbo.employee_loan", "LoanId", "dbo.loan", "LoanId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.employee_loan", "LoanId", "dbo.loan");
            DropIndex("dbo.employee_loan", new[] { "LoanId" });
            AlterColumn("dbo.employee_loan", "MonthlyPaymentDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.employee_loan", "BiMonthlyPaymentSecondDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.employee_loan", "BiMonthlyPaymentFirstDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.employee_loan", "WeeklyPaymentDayOfWeek", c => c.DateTime(nullable: false, precision: 0));
        }
    }
}
