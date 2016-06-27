namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adjustment_changes : DbMigration
    {
        public override void Up()
        {
            //DropPrimaryKey("employee_adjustment");
            //CreateTable(
            //    "dbo.Adjustments",
            //    c => new
            //        {
            //            AdjustmentId = c.Int(nullable: false, identity: true),
            //            AdjustmentName = c.String(unicode: false),
            //            Remarks = c.String(unicode: false),
            //            IsActive = c.Boolean(nullable: false),
            //            CreateDate = c.DateTime(nullable: false, precision: 0),
            //            UpdateDate = c.DateTime(precision: 0),
            //        })
            //    .PrimaryKey(t => t.AdjustmentId);
            
            //AddColumn("dbo.employee_adjustment", "EmployeeAdjustmentId", c => c.Int(nullable: false, identity: true));
            //AlterColumn("dbo.deduction", "DeductionName", c => c.String(nullable: false, maxLength: 50, storeType: "nvarchar"));
            //AlterColumn("dbo.employee_adjustment", "AdjustmentId", c => c.Int(nullable: false));
            //AddPrimaryKey("dbo.employee_adjustment", "EmployeeAdjustmentId");
            //CreateIndex("dbo.employee_adjustment", "AdjustmentId");
            //CreateIndex("dbo.employee_adjustment", "EmployeeId");
            //AddForeignKey("dbo.employee_adjustment", "AdjustmentId", "dbo.Adjustments", "AdjustmentId", cascadeDelete: true);
            //AddForeignKey("dbo.employee_adjustment", "EmployeeId", "dbo.employee", "EmployeeId", cascadeDelete: true);
            //DropColumn("dbo.employee_adjustment", "AdjustmentTypeId");


            DropTable("employee_adjustment");
            CreateTable(
                "dbo.Adjustments",
                c => new
                {
                    AdjustmentId = c.Int(nullable: false, identity: true),
                    AdjustmentName = c.String(unicode: false),
                    Description = c.String(unicode: false),
                    IsActive = c.Boolean(nullable: false),
                    CreateDate = c.DateTime(nullable: false, precision: 0),
                    UpdateDate = c.DateTime(precision: 0),
                })
                .PrimaryKey(t => t.AdjustmentId);

            CreateTable(
                "dbo.employee_adjustment",
                c => new
                {
                    EmployeeAdjustmentId = c.Int(nullable: false, identity: true),
                    AdjustmentId = c.Int(nullable: false),
                    EmployeeId = c.Int(nullable: false),
                    Date = c.DateTime(nullable: false, precision: 0),
                    Amount = c.Decimal(),
                    Remarks = c.String(unicode: false),
                    IsActive = c.Boolean(nullable: false),
                    CreateDate = c.DateTime(nullable: false, precision: 0),
                    UpdateDate = c.DateTime(precision: 0),
                }).PrimaryKey(t => t.EmployeeAdjustmentId);

            AlterColumn("dbo.deduction", "DeductionName", c => c.String(nullable: false, maxLength: 50, storeType: "nvarchar"));
            CreateIndex("dbo.employee_adjustment", "AdjustmentId");
            CreateIndex("dbo.employee_adjustment", "EmployeeId");
            AddForeignKey("dbo.employee_adjustment", "AdjustmentId", "dbo.Adjustments", "AdjustmentId", cascadeDelete: true);
            AddForeignKey("dbo.employee_adjustment", "EmployeeId", "dbo.employee", "EmployeeId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            AddColumn("dbo.employee_adjustment", "AdjustmentTypeId", c => c.Int(nullable: false));
            DropForeignKey("dbo.employee_adjustment", "EmployeeId", "dbo.employee");
            DropForeignKey("dbo.employee_adjustment", "AdjustmentId", "dbo.Adjustments");
            DropIndex("dbo.employee_adjustment", new[] { "EmployeeId" });
            DropIndex("dbo.employee_adjustment", new[] { "AdjustmentId" });
            DropPrimaryKey("dbo.employee_adjustment");
            AlterColumn("dbo.employee_adjustment", "AdjustmentId", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.deduction", "DeductionName", c => c.String(maxLength: 50, storeType: "nvarchar"));
            DropColumn("dbo.employee_adjustment", "EmployeeAdjustmentId");
            DropTable("dbo.Adjustments");
            AddPrimaryKey("dbo.employee_adjustment", "AdjustmentId");
        }
    }
}
