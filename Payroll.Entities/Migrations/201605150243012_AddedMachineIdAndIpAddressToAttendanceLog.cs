namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMachineIdAndIpAddressToAttendanceLog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.attendance_log", "MachineId", c => c.Int(nullable: false));
            AddColumn("dbo.attendance_log", "IpAddress", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.attendance_log", "IpAddress");
            DropColumn("dbo.attendance_log", "MachineId");
        }
    }
}
