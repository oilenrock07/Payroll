namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmployeeMachineEntitiesChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.employee_machine", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.employee_machine", "CreateDate", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.employee_machine", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.machines", "IpAddress", c => c.String(nullable: false, unicode: false));
            CreateIndex("dbo.employee_machine", "MachineId");
            AddForeignKey("dbo.employee_machine", "MachineId", "dbo.machines", "MachineId", cascadeDelete: true);
            DropColumn("dbo.employee", "EnrolledToRfid");
            DropColumn("dbo.employee", "EnrolledToBiometrics");
        }
        
        public override void Down()
        {
            AddColumn("dbo.employee", "EnrolledToBiometrics", c => c.Boolean(nullable: false));
            AddColumn("dbo.employee", "EnrolledToRfid", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.employee_machine", "MachineId", "dbo.machines");
            DropIndex("dbo.employee_machine", new[] { "MachineId" });
            AlterColumn("dbo.machines", "IpAddress", c => c.String(unicode: false));
            DropColumn("dbo.employee_machine", "UpdateDate");
            DropColumn("dbo.employee_machine", "CreateDate");
            DropColumn("dbo.employee_machine", "IsActive");
        }
    }
}
