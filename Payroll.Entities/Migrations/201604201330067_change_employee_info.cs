namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class change_employee_info : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.employee_info", "EmployeeSalaryId", "dbo.employee_salary");
            DropIndex("dbo.employee_info", new[] { "EmployeeSalaryId" });
            AlterColumn("dbo.employee_info", "EmployeeSalaryId", c => c.Int());
            CreateIndex("dbo.employee_info", "EmployeeSalaryId");
            AddForeignKey("dbo.employee_info", "EmployeeSalaryId", "dbo.employee_salary", "EmploymentSalaryId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.employee_info", "EmployeeSalaryId", "dbo.employee_salary");
            DropIndex("dbo.employee_info", new[] { "EmployeeSalaryId" });
            AlterColumn("dbo.employee_info", "EmployeeSalaryId", c => c.Int(nullable: false));
            CreateIndex("dbo.employee_info", "EmployeeSalaryId");
            AddForeignKey("dbo.employee_info", "EmployeeSalaryId", "dbo.employee_salary", "EmploymentSalaryId", cascadeDelete: true);
        }
    }
}
