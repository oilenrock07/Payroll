namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GenderAndNickNameInEmployee : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.employee_info", "DateHired", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.employee_info", "EmploymentStatus", c => c.Int(nullable: false));
            AddColumn("dbo.employee_info", "CustomDate1", c => c.DateTime(precision: 0));
            AddColumn("dbo.employee_info", "CustomDate2", c => c.DateTime(precision: 0));
            AddColumn("dbo.employee_info", "CustomString1", c => c.String(maxLength: 250, storeType: "nvarchar"));
            AddColumn("dbo.employee_info", "CustomString2", c => c.String(maxLength: 250, storeType: "nvarchar"));
            AddColumn("dbo.employee_info", "CustomDecimal1", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.employee_info", "CustomDecimal2", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.employee", "NickName", c => c.String(maxLength: 100, storeType: "nvarchar"));
            AddColumn("dbo.employee", "Gender", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.employee", "Gender");
            DropColumn("dbo.employee", "NickName");
            DropColumn("dbo.employee_info", "CustomDecimal2");
            DropColumn("dbo.employee_info", "CustomDecimal1");
            DropColumn("dbo.employee_info", "CustomString2");
            DropColumn("dbo.employee_info", "CustomString1");
            DropColumn("dbo.employee_info", "CustomDate2");
            DropColumn("dbo.employee_info", "CustomDate1");
            DropColumn("dbo.employee_info", "EmploymentStatus");
            DropColumn("dbo.employee_info", "DateHired");
        }
    }
}
