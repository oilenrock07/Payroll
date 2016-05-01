namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class employeemachine : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.employee_machine",
                c => new
                    {
                        EmployeeMachineId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        MachineId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeMachineId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.employee_machine");
        }
    }
}
