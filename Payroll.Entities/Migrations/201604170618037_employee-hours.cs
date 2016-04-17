namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class employeehours : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Employee_Daily_Payroll",
                c => new
                    {
                        EmployeeDailySalaryId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        TotalEmployeeHoursId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false, precision: 0),
                        TotalPay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsActive = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false, precision: 0),
                        UpdateDate = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => t.EmployeeDailySalaryId);
            
            AddColumn("dbo.employee_hours", "IsIncludedInTotal", c => c.Boolean(nullable: false));
            AddColumn("dbo.payroll", "TotalNet", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.payroll", "TotalGross", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.payroll", "Salary");
            DropColumn("dbo.payroll", "TotalPay");
        }
        
        public override void Down()
        {
            AddColumn("dbo.payroll", "TotalPay", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.payroll", "Salary", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.payroll", "TotalGross");
            DropColumn("dbo.payroll", "TotalNet");
            DropColumn("dbo.employee_hours", "IsIncludedInTotal");
            DropTable("dbo.Employee_Daily_Payroll");
        }
    }
}
