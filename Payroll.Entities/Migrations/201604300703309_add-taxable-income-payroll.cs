namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtaxableincomepayroll : DbMigration
    {
        public override void Up()
        {
            AddColumn("payroll", "TaxableIncome", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("payroll", "TaxableIncome");
        }
    }
}
