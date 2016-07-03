namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class payrollitems : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.employee_payroll_item",
                c => new
                    {
                        EmployeePayrollItemId = c.Int(nullable: false, identity: true),
                        RateType = c.Int(nullable: false),
                        PayrollId = c.Int(nullable: false),
                        TotalHours = c.Double(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsActive = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false, precision: 0),
                        UpdateDate = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => t.EmployeePayrollItemId)
                .ForeignKey("dbo.payroll", t => t.PayrollId, cascadeDelete: true)
                .Index(t => t.PayrollId);
            
            CreateIndex("dbo.employee_daily_payroll", "TotalEmployeeHoursId");
            AddForeignKey("dbo.employee_daily_payroll", "TotalEmployeeHoursId", "dbo.employee_hours_total", "TotalEmployeeHoursId");
            DropColumn("dbo.payroll", "TotalRegularHours");
            DropColumn("dbo.payroll", "TotalOTHours");
            DropColumn("dbo.payroll", "TotalRestDayHours");
            DropColumn("dbo.payroll", "TotalNightDifHours");
            DropColumn("dbo.payroll", "TotalRegular");
            DropColumn("dbo.payroll", "TotalOT");
            DropColumn("dbo.payroll", "TotalRestDay");
            DropColumn("dbo.payroll", "TotalNightDif");
        }
        
        public override void Down()
        {
            AddColumn("dbo.payroll", "TotalNightDif", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.payroll", "TotalRestDay", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.payroll", "TotalOT", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.payroll", "TotalRegular", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.payroll", "TotalNightDifHours", c => c.Double(nullable: false));
            AddColumn("dbo.payroll", "TotalRestDayHours", c => c.Double(nullable: false));
            AddColumn("dbo.payroll", "TotalOTHours", c => c.Double(nullable: false));
            AddColumn("dbo.payroll", "TotalRegularHours", c => c.Double(nullable: false));
            DropForeignKey("dbo.employee_payroll_item", "PayrollId", "dbo.payroll");
            DropForeignKey("dbo.employee_daily_payroll", "TotalEmployeeHoursId", "dbo.employee_hours_total");
            DropIndex("dbo.employee_payroll_item", new[] { "PayrollId" });
            DropIndex("dbo.employee_daily_payroll", new[] { "TotalEmployeeHoursId" });
            DropTable("dbo.employee_payroll_item");
        }
    }
}
