namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.employee_hours_total_per_company",
                c => new
                    {
                        TotalEmployeeHoursPerCompanyId = c.Int(nullable: false, identity: true),
                        TotalEmployeeHoursId = c.Int(nullable: false),
                        CompanyId = c.Int(nullable: false),
                        Hours = c.Double(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.TotalEmployeeHoursPerCompanyId)
                .ForeignKey("dbo.company", t => t.CompanyId, cascadeDelete: true)
                .ForeignKey("dbo.employee_hours_total", t => t.TotalEmployeeHoursId, cascadeDelete: true)
                .Index(t => t.TotalEmployeeHoursId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.company",
                c => new
                    {
                        CompanyId = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(maxLength: 100),
                        CompanyCode = c.String(maxLength: 20),
                        Address = c.String(maxLength: 250),
                        CompanyInfo = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.CompanyId);
            
            AddColumn("dbo.employee_leave", "StartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.employee_leave", "EndDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.leave", "IsPayable", c => c.Boolean(nullable: false));
            AddColumn("dbo.leave", "IsHolidayAfterPayable", c => c.Boolean(nullable: false));
            AddColumn("dbo.holiday", "IsAlwaysPayable", c => c.Boolean(nullable: false));
            CreateIndex("dbo.employee_hours_total", "EmployeeId");
            AddForeignKey("dbo.employee_hours_total", "EmployeeId", "dbo.employee", "EmployeeId", cascadeDelete: true);
            DropColumn("dbo.employee_leave", "Date");
        }
        
        public override void Down()
        {
            AddColumn("dbo.employee_leave", "Date", c => c.DateTime(nullable: false));
            DropForeignKey("dbo.employee_hours_total_per_company", "TotalEmployeeHoursId", "dbo.employee_hours_total");
            DropForeignKey("dbo.employee_hours_total_per_company", "CompanyId", "dbo.company");
            DropForeignKey("dbo.employee_hours_total", "EmployeeId", "dbo.employee");
            DropIndex("dbo.employee_hours_total_per_company", new[] { "CompanyId" });
            DropIndex("dbo.employee_hours_total_per_company", new[] { "TotalEmployeeHoursId" });
            DropIndex("dbo.employee_hours_total", new[] { "EmployeeId" });
            DropColumn("dbo.holiday", "IsAlwaysPayable");
            DropColumn("dbo.leave", "IsHolidayAfterPayable");
            DropColumn("dbo.leave", "IsPayable");
            DropColumn("dbo.employee_leave", "EndDate");
            DropColumn("dbo.employee_leave", "StartDate");
            DropTable("dbo.company");
            DropTable("dbo.employee_hours_total_per_company");
        }
    }
}
