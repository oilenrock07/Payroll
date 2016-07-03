namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class additionalfieldspayroll : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.payroll", "TotalRegularHours", c => c.Double(nullable: false));
            AddColumn("dbo.payroll", "TotalOTHours", c => c.Double(nullable: false));
            AddColumn("dbo.payroll", "TotalRestDayHours", c => c.Double(nullable: false));
            AddColumn("dbo.payroll", "TotalNightDifHours", c => c.Double(nullable: false));
            AddColumn("dbo.payroll", "TotalRegular", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.payroll", "TotalOT", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.payroll", "TotalRestDay", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.payroll", "TotalNightDif", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.payroll", "TotalNightDif");
            DropColumn("dbo.payroll", "TotalRestDay");
            DropColumn("dbo.payroll", "TotalOT");
            DropColumn("dbo.payroll", "TotalRegular");
            DropColumn("dbo.payroll", "TotalNightDifHours");
            DropColumn("dbo.payroll", "TotalRestDayHours");
            DropColumn("dbo.payroll", "TotalOTHours");
            DropColumn("dbo.payroll", "TotalRegularHours");
        }
    }
}
