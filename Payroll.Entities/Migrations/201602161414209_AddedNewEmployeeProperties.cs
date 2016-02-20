namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNewEmployeeProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.employee", "EnrolledToRfid", c => c.Boolean(nullable: false));
            AddColumn("dbo.employee", "EnrolledToBiometrics", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.employee", "EnrolledToBiometrics");
            DropColumn("dbo.employee", "EnrolledToRfid");
        }
    }
}
