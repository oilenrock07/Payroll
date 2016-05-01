namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addemployeepayrollistaxed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.payroll", "IsTaxed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.payroll", "IsTaxed");
        }
    }
}
