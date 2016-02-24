namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUserEntities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        Id = c.String(maxLength:250),
                        Name = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(maxLength: 500),
                        ClaimValue = c.String(maxLength: 500),
                        User_Id = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        UserLoginId = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(maxLength: 250),
                        LoginProvider = c.String(maxLength: 500),
                        ProviderKey = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.UserLoginId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserRoleId = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 250),
                        RoleId = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.UserRoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(maxLength: 250),
                        PasswordHash = c.String(maxLength: 500),
                        SecurityStamp = c.String(maxLength: 500),
                        Discriminator = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetRoles");
        }
    }
}
