namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDescriptionToHoliday : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.holiday", "Description", c => c.String(maxLength: 500, storeType: "nvarchar"));
            AlterColumn("dbo.holiday", "HolidayName", c => c.String(nullable: false, maxLength: 50, storeType: "nvarchar"));
            AlterColumn("dbo.work_schedule", "TimeStart", c => c.Time(nullable: false, precision: 0));
            AlterColumn("dbo.work_schedule", "TimeEnd", c => c.Time(nullable: false, precision: 0));
            CreateIndex("dbo.employee_workschedule", "WorkScheduleId");
            AddForeignKey("dbo.employee_workschedule", "WorkScheduleId", "dbo.work_schedule", "WorkScheduleId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.employee_workschedule", "WorkScheduleId", "dbo.work_schedule");
            DropIndex("dbo.employee_workschedule", new[] { "WorkScheduleId" });
            AlterColumn("dbo.work_schedule", "TimeEnd", c => c.Int(nullable: false));
            AlterColumn("dbo.work_schedule", "TimeStart", c => c.Int(nullable: false));
            AlterColumn("dbo.holiday", "HolidayName", c => c.String(maxLength: 50, storeType: "nvarchar"));
            DropColumn("dbo.holiday", "Description");
        }
    }
}
