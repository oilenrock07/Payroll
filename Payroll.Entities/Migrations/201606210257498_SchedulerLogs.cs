namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SchedulerLogs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SchedulerLogs",
                c => new
                    {
                        SchedulerLogId = c.Int(nullable: false, identity: true),
                        ScheduleType = c.String(unicode: false),
                        LogType = c.Int(nullable: false),
                        Exception = c.String(unicode: false),
                        IsActive = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false, precision: 0),
                        UpdateDate = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => t.SchedulerLogId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SchedulerLogs");
        }
    }
}
