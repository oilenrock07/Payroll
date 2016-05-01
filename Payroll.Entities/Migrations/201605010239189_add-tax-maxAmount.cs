namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtaxmaxAmount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tax", "MaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tax", "MaxAmount");
        }
    }
}
