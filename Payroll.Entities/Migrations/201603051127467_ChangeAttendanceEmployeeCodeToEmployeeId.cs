namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeAttendanceEmployeeCodeToEmployeeId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.attendance_log", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("dbo.attendance", "EmployeeId", c => c.Int(nullable: false));
            DropColumn("dbo.attendance_log", "EmployeeCode");
            DropColumn("dbo.attendance", "EmployeeCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.attendance", "EmployeeCode", c => c.String(maxLength: 50, storeType: "nvarchar"));
            AddColumn("dbo.attendance_log", "EmployeeCode", c => c.String(maxLength: 50, storeType: "nvarchar"));
            DropColumn("dbo.attendance", "EmployeeId");
            DropColumn("dbo.attendance_log", "EmployeeId");
        }
    }
}
