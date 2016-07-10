namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class payrollitemschanges : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("employee_payroll_item", "PayrollId", "payroll");
            //DropIndex("employee_payroll_item", new[] { "PayrollId" });
            //AddColumn("employee_payroll_item", "PayrollDate", c => c.DateTime(nullable: false, precision: 0));
            //AddColumn("employee_payroll_item", "EmployeeId", c => c.Int(nullable: false));
            //AddColumn("employee_payroll_item", "Multiplier", c => c.Double(nullable: false));
            //AddColumn("employee_payroll_item", "RatePerHour", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            //AlterColumn("employee_payroll_item", "PayrollId", c => c.Int());
            //CreateIndex("employee_payroll_item", "PayrollId");
            //AddForeignKey("employee_payroll_item", "PayrollId", "payroll", "PayrollId");
        }
        
        public override void Down()
        {
            DropForeignKey("employee_payroll_item", "PayrollId", "payroll");
            DropIndex("employee_payroll_item", new[] { "PayrollId" });
            AlterColumn("employee_payroll_item", "PayrollId", c => c.Int(nullable: false));
            DropColumn("employee_payroll_item", "RatePerHour");
            DropColumn("employee_payroll_item", "Multiplier");
            DropColumn("employee_payroll_item", "EmployeeId");
            DropColumn("employee_payroll_item", "PayrollDate");
            CreateIndex("employee_payroll_item", "PayrollId");
            AddForeignKey("employee_payroll_item", "PayrollId", "payroll", "PayrollId", cascadeDelete: true);
        }
    }
}
