namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cleaninguprepeatedfields : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.employee", "Enabled");
            DropColumn("dbo.employee_workschedule", "DateCreated");
        }
        
        public override void Down()
        {
            AddColumn("dbo.employee_workschedule", "DateCreated", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.employee", "Enabled", c => c.Boolean(nullable: false));
        }
    }
}
