namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeemployeedailypayroll : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.employee_daily_payroll", "TotalEmployeeHoursId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.employee_daily_payroll", "TotalEmployeeHoursId", c => c.Int(nullable: false));
        }
    }
}
