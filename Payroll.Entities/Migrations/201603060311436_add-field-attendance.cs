namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addfieldattendance : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.attendance_log", "IsConsidered", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.attendance_log", "IsConsidered");
        }
    }
}
