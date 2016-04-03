namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class baseentity : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.attendance_log", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.attendance", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.deduction", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.department_manager", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.department", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.employee_adjustment", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.employee_department", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.employee_files", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.employee_hours", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.employee_info", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.employee", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.employee_leave", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.employee_loan", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.loan", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.employee_workschedule", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.work_schedule", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.files", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.frequency", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.holiday", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.leave", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.loan_payment", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.payment_frequency", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.payroll", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.positions", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.schedule", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.settings", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.tax", "UpdateDate", c => c.DateTime(precision: 0));
            AlterColumn("dbo.employee_hours_total", "UpdateDate", c => c.DateTime(precision: 0));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.employee_hours_total", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.tax", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.settings", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.schedule", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.positions", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.payroll", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.payment_frequency", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.loan_payment", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.leave", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.holiday", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.frequency", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.files", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.work_schedule", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.employee_workschedule", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.loan", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.employee_loan", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.employee_leave", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.employee", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.employee_info", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.employee_hours", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.employee_files", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.employee_department", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.employee_adjustment", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.department", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.department_manager", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.deduction", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.attendance", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.attendance_log", "UpdateDate", c => c.DateTime(nullable: false, precision: 0));
        }
    }
}
