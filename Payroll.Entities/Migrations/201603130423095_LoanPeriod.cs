namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LoanPeriod : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.loan", "LoanPeriod", c => c.Int(nullable: false));
            AddColumn("dbo.loan", "LoanPeriodNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.loan", "LoanPeriodNumber");
            DropColumn("dbo.loan", "LoanPeriod");
        }
    }
}
