namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsActiveToEmployee : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.employee", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.employee", "IsActive");
        }
    }
}
