namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adjustmenttype : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Adjustments", "AdjustmentType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Adjustments", "AdjustmentType");
        }
    }
}
