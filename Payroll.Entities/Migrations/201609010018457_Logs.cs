namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Logs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.logs",
                c => new
                    {
                        LogId = c.Int(nullable: false, identity: true),
                        DateLogged = c.DateTime(nullable: false),
                        Level = c.String(maxLength: 15),
                        UserName = c.String(),
                        Message = c.String(),
                        Url = c.String(),
                        Logger = c.String(),
                        Callsite = c.String(),
                        Exception = c.String(),
                    })
                .PrimaryKey(t => t.LogId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.logs");
        }
    }
}
