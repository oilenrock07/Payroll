namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class additionalattendancefields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.attendance", "IsManuallyEdited", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.attendance", "IsManuallyEdited");
        }
    }
}
