namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class machineentities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.machines",
                c => new
                    {
                        MachineId = c.Int(nullable: false, identity: true),
                        IpAddress = c.String(unicode: false),
                        IsActive = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false, precision: 0),
                        UpdateDate = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => t.MachineId);
            
            AddColumn("dbo.employee_payroll_deduction", "EmployeeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.employee_payroll_deduction", "EmployeeId");
            DropTable("dbo.machines");
        }
    }
}
