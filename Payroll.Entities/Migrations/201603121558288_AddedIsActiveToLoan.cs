namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsActiveToLoan : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.employee_hours", "OriginAttendanceId", c => c.Int(nullable: false));
            AddColumn("dbo.loan", "IsActive", c => c.Boolean(nullable: false));
            AlterColumn("dbo.employee_hours", "Hours", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.employee_hours", "Hours", c => c.Int(nullable: false));
            DropColumn("dbo.loan", "IsActive");
            DropColumn("dbo.employee_hours", "OriginAttendanceId");
        }
    }
}
