namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InheritUserEntitiesToBaseClass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "CreateDate", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.AspNetUsers", "UpdateDate", c => c.DateTime(precision: 0));
            AddColumn("dbo.AspNetRoles", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetRoles", "CreateDate", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.AspNetRoles", "UpdateDate", c => c.DateTime(precision: 0));
            AddColumn("dbo.AspNetUserClaims", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUserClaims", "CreateDate", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.AspNetUserClaims", "UpdateDate", c => c.DateTime(precision: 0));
            AddColumn("dbo.AspNetUserLogins", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUserLogins", "CreateDate", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.AspNetUserLogins", "UpdateDate", c => c.DateTime(precision: 0));
            AddColumn("dbo.AspNetUserRoles", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUserRoles", "CreateDate", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.AspNetUserRoles", "UpdateDate", c => c.DateTime(precision: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUserRoles", "UpdateDate");
            DropColumn("dbo.AspNetUserRoles", "CreateDate");
            DropColumn("dbo.AspNetUserRoles", "IsActive");
            DropColumn("dbo.AspNetUserLogins", "UpdateDate");
            DropColumn("dbo.AspNetUserLogins", "CreateDate");
            DropColumn("dbo.AspNetUserLogins", "IsActive");
            DropColumn("dbo.AspNetUserClaims", "UpdateDate");
            DropColumn("dbo.AspNetUserClaims", "CreateDate");
            DropColumn("dbo.AspNetUserClaims", "IsActive");
            DropColumn("dbo.AspNetRoles", "UpdateDate");
            DropColumn("dbo.AspNetRoles", "CreateDate");
            DropColumn("dbo.AspNetRoles", "IsActive");
            DropColumn("dbo.AspNetUsers", "UpdateDate");
            DropColumn("dbo.AspNetUsers", "CreateDate");
            DropColumn("dbo.AspNetUsers", "IsActive");
        }
    }
}
