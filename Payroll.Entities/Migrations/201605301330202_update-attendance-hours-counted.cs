namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateattendancehourscounted : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.attendance", "IsHoursCounted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.attendance", "IsHoursCounted");
        }
    }
}
