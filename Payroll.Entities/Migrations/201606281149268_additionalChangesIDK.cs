namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class additionalChangesIDK : DbMigration
    {
        public override void Up()
        {
          //  AddColumn("dbo.Adjustments", "Description", c => c.String(unicode: false));
            //AlterColumn("dbo.Adjustments", "AdjustmentName", c => c.String(nullable: false, unicode: false));
            //DropColumn("dbo.Adjustments", "Remarks");
        }
        
        public override void Down()
        {
            //AddColumn("dbo.Adjustments", "Remarks", c => c.String(unicode: false));
           // AlterColumn("dbo.Adjustments", "AdjustmentName", c => c.String(unicode: false));
           // DropColumn("dbo.Adjustments", "Description");
        }
    }
}
