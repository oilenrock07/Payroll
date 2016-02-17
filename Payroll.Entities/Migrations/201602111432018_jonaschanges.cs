namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class jonaschanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.employee_hours",
                c => new
                    {
                        EmployeeHoursId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Hours = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeHoursId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.employee_hours");
        }
    }
}
