namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class payment_frequency : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("payment_frequency", "FrequencyId", "frequency");
            DropIndex("payment_frequency", new[] { "FrequencyId" });
            AlterColumn("payment_frequency", "FrequencyId", c => c.Int());
            CreateIndex("payment_frequency", "FrequencyId");
            AddForeignKey("payment_frequency", "FrequencyId", "frequency", "FrequencyId");
        }
        
        public override void Down()
        {
            DropForeignKey("payment_frequency", "FrequencyId", "frequency");
            DropIndex("payment_frequency", new[] { "FrequencyId" });
            AlterColumn("payment_frequency", "FrequencyId", c => c.Int(nullable: false));
            CreateIndex("payment_frequency", "FrequencyId");
            AddForeignKey("payment_frequency", "FrequencyId", "frequency", "FrequencyId", cascadeDelete: true);
        }
    }
}
