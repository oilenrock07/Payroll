namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class employeedailypayroll : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("employee_daily_payroll", "TotalEmployeeHoursId", "employee_hours_total");
            DropIndex("employee_daily_payroll", new[] { "TotalEmployeeHoursId" });
        }
        
        public override void Down()
        {
            CreateIndex("employee_daily_payroll", "TotalEmployeeHoursId");
            AddForeignKey("employee_daily_payroll", "TotalEmployeeHoursId", "employee_hours_total", "TotalEmployeeHoursId");
        }
    }
}
