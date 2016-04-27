namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration20160427 : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("employee_info", "EmployeeSalaryId", "employee_salary");
            //DropIndex("employee_info", new[] { "EmployeeSalaryId" });
            //RenameColumn(table: "employee_leave", name: "ApprovedBy", newName: "Id");
            CreateTable(
                "deduction_amount",
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
            
            AddColumn("AspNetUsers", "FirstName", c => c.String(unicode: false));
            AddColumn("AspNetUsers", "LastName", c => c.String(unicode: false));
            AddColumn("employee_leave", "Id", c => c.String(maxLength: 128, storeType: "nvarchar"));
            AlterColumn("employee_info", "EmployeeSalaryId", c => c.Int());
            //AlterColumn("employee_leave", "Id", c => c.String(maxLength: 128, storeType: "nvarchar"));
            CreateIndex("employee_info", "EmployeeSalaryId");
            CreateIndex("employee_leave", "LeaveId");
            CreateIndex("employee_leave", "Id");
            AddForeignKey("employee_leave", "LeaveId", "leave", "LeaveId", cascadeDelete: true);
            AddForeignKey("employee_leave", "Id", "AspNetUsers", "Id");
            AddForeignKey("employee_info", "EmployeeSalaryId", "employee_salary", "EmploymentSalaryId");
        }
        
        public override void Down()
        {
            //DropForeignKey("employee_info", "EmployeeSalaryId", "employee_salary");
            DropForeignKey("employee_leave", "Id", "AspNetUsers");
            DropForeignKey("employee_leave", "LeaveId", "leave");
            DropIndex("employee_leave", new[] { "Id" });
            DropIndex("employee_leave", new[] { "LeaveId" });
            DropIndex("employee_info", new[] { "EmployeeSalaryId" });
            AlterColumn("employee_leave", "Id", c => c.Int(nullable: false));
            AlterColumn("employee_info", "EmployeeSalaryId", c => c.Int(nullable: false));
            DropColumn("AspNetUsers", "LastName");
            DropColumn("AspNetUsers", "FirstName");
            DropTable("deduction_amount");
            DropColumn("employee_leave", "ApprovedBy");
            //RenameColumn(table: "employee_leave", name: "Id", newName: "ApprovedBy");
            CreateIndex("employee_info", "EmployeeSalaryId");
            AddForeignKey("employee_info", "EmployeeSalaryId", "employee_salary", "EmploymentSalaryId", cascadeDelete: true);
        }
    }
}
