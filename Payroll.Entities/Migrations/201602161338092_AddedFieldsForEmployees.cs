namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedFieldsForEmployees : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.employee", "Privilege", c => c.Int(nullable: false));
            AddColumn("dbo.employee", "Enabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.employee", "Enabled");
            DropColumn("dbo.employee", "Privilege");
        }
    }
}
