namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newmigration : DbMigration
    {
        public override void Up()
        {
            //RenameColumn(table: "dbo.employee_leave", name: "ApprovedBy", newName: "Id");
            CreateTable(
                "dbo.deduction_amount",
                c => new
                    {
                        DeductionAmountId = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 50, storeType: "nvarchar"),
                        DeductionId = c.Int(nullable: false),
                        Frequency = c.Int(nullable: false),
                        MinBaseAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaxBaseAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsPercentage = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false, precision: 0),
                        UpdateDate = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => t.DeductionAmountId);
            
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
            
            AddColumn("dbo.employee_info", "EmployeeSalaryId", c => c.Int());
            AddColumn("dbo.frequency", "FrequencyType", c => c.Int());
            AddColumn("dbo.AspNetUsers", "FirstName", c => c.String(unicode: false));
            AddColumn("dbo.AspNetUsers", "LastName", c => c.String(unicode: false));
            AlterColumn("dbo.employee_leave", "Id", c => c.String(maxLength: 128, storeType: "nvarchar"));
            CreateIndex("dbo.employee_info", "PaymentFrequencyId");
            CreateIndex("dbo.employee_info", "EmployeeSalaryId");
            CreateIndex("dbo.employee_leave", "LeaveId");
            CreateIndex("dbo.employee_leave", "Id");
            AddForeignKey("dbo.employee_info", "EmployeeSalaryId", "dbo.employee_salary", "EmploymentSalaryId");
            AddForeignKey("dbo.employee_info", "PaymentFrequencyId", "dbo.payment_frequency", "PaymentFrequencyId");
            AddForeignKey("dbo.employee_leave", "LeaveId", "dbo.leave", "LeaveId", cascadeDelete: true);
            AddForeignKey("dbo.employee_leave", "Id", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.employee_info", "Salary");
        }
        
        public override void Down()
        {
            AddColumn("dbo.employee_info", "Salary", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropForeignKey("dbo.employee_leave", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.employee_leave", "LeaveId", "dbo.leave");
            DropForeignKey("dbo.employee_info", "PaymentFrequencyId", "dbo.payment_frequency");
            DropForeignKey("dbo.employee_info", "EmployeeSalaryId", "dbo.employee_salary");
            DropIndex("dbo.employee_leave", new[] { "Id" });
            DropIndex("dbo.employee_leave", new[] { "LeaveId" });
            DropIndex("dbo.employee_info", new[] { "EmployeeSalaryId" });
            DropIndex("dbo.employee_info", new[] { "PaymentFrequencyId" });
            AlterColumn("dbo.employee_leave", "Id", c => c.Int(nullable: false));
            DropColumn("dbo.AspNetUsers", "LastName");
            DropColumn("dbo.AspNetUsers", "FirstName");
            DropColumn("dbo.frequency", "FrequencyType");
            DropColumn("dbo.employee_info", "EmployeeSalaryId");
            DropTable("dbo.employee_salary");
            DropTable("dbo.deduction_amount");
            //RenameColumn(table: "dbo.employee_leave", name: "Id", newName: "ApprovedBy");
        }
    }
}
