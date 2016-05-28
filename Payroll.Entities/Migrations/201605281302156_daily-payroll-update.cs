namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dailypayrollupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.employee_daily_payroll", "RateType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.employee_daily_payroll", "RateType");
        }
    }
}
