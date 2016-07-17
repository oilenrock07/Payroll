namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class logindisplayclient : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LogInDisplayClients",
                c => new
                    {
                        LogInDisplayClientId = c.Int(nullable: false, identity: true),
                        IpAddress = c.String(unicode: false),
                        ClientId = c.String(unicode: false),
                        IsActive = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false, precision: 0),
                        UpdateDate = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => t.LogInDisplayClientId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.LogInDisplayClients");
        }
    }
}
