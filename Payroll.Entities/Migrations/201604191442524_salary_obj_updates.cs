namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salary_obj_updates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.employee_salary",
                c => new
                    {
                        EmploymentSalaryId = c.Int(nullable: false, identity: true),
                        Salary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SalaryFrequency = c.Int(nullable: false),
                        EmployeeInfoId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false, precision: 0),
                        UpdateDate = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => t.EmploymentSalaryId);
            
            AddColumn("dbo.employee_info", "EmployeeSalaryId", c => c.Int(nullable: false));
            AddColumn("dbo.frequency", "FrequencyType", c => c.Int());
            CreateIndex("dbo.employee_info", "PaymentFrequencyId");
            CreateIndex("dbo.employee_info", "EmployeeSalaryId");
            AddForeignKey("dbo.employee_info", "EmployeeSalaryId", "dbo.employee_salary", "EmploymentSalaryId", cascadeDelete: true);
            AddForeignKey("dbo.employee_info", "PaymentFrequencyId", "dbo.payment_frequency", "PaymentFrequencyId");
            DropColumn("dbo.employee_info", "Salary");
        }
        
        public override void Down()
        {
            AddColumn("dbo.employee_info", "Salary", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropForeignKey("dbo.employee_info", "PaymentFrequencyId", "dbo.payment_frequency");
            DropForeignKey("dbo.employee_info", "EmployeeSalaryId", "dbo.employee_salary");
            DropIndex("dbo.employee_info", new[] { "EmployeeSalaryId" });
            DropIndex("dbo.employee_info", new[] { "PaymentFrequencyId" });
            DropColumn("dbo.frequency", "FrequencyType");
            DropColumn("dbo.employee_info", "EmployeeSalaryId");
            DropTable("dbo.employee_salary");
        }
    }
}
