namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dbupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.employee_salary", "EmployeeId", c => c.Int(nullable: false));
            DropColumn("dbo.employee_salary", "EmployeeInfoId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.employee_salary", "EmployeeInfoId", c => c.Int(nullable: false));
            DropColumn("dbo.employee_salary", "EmployeeId");
        }
    }
}
