namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeFrequencyIdandpositionidnullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.employee_info", "PaymentFrequencyId", c => c.Int());
            AlterColumn("dbo.employee_info", "PositionId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.employee_info", "PositionId", c => c.Int(nullable: false));
            AlterColumn("dbo.employee_info", "PaymentFrequencyId", c => c.Int(nullable: false));
        }
    }
}
