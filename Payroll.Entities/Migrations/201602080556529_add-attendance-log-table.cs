namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addattendancelogtable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.attendance_log",
                c => new
                    {
                        AttendanceLogId = c.Int(nullable: false, identity: true),
                        EmployeeCode = c.String(maxLength: 50, storeType: "nvarchar"),
                        ClockInOut = c.DateTime(nullable: false, precision: 0),
                        Type = c.Int(nullable: false),
                        IsRecorded = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.AttendanceLogId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.attendance_log");
        }
    }
}
