namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsActiveStatusInEmployeeDepartment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.employee_department", "IsActive", c => c.Boolean(nullable: false));
            AlterColumn("dbo.department", "DepartmentName", c => c.String(nullable: false, maxLength: 250, storeType: "nvarchar"));
            AlterColumn("dbo.employee", "FirstName", c => c.String(nullable: false, maxLength: 50, storeType: "nvarchar"));
            AlterColumn("dbo.employee", "LastName", c => c.String(nullable: false, maxLength: 50, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.employee", "LastName", c => c.String(maxLength: 50, storeType: "nvarchar"));
            AlterColumn("dbo.employee", "FirstName", c => c.String(maxLength: 50, storeType: "nvarchar"));
            AlterColumn("dbo.department", "DepartmentName", c => c.String(maxLength: 250, storeType: "nvarchar"));
            DropColumn("dbo.employee_department", "IsActive");
        }
    }
}
