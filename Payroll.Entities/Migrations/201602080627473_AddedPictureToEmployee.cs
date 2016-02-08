namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPictureToEmployee : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.employee", "Picture", c => c.String());
            AlterColumn("dbo.attendance_log", "ClockInOut", c => c.DateTime(nullable: false));
            AlterColumn("dbo.attendance", "ClockIn", c => c.DateTime(nullable: false));
            AlterColumn("dbo.attendance", "ClockOut", c => c.DateTime());
            AlterColumn("dbo.deduction", "Remarks", c => c.String());
            AlterColumn("dbo.employee_adjustment", "Date", c => c.DateTime(nullable: false));
            AlterColumn("dbo.employee_adjustment", "Remarks", c => c.String());
            AlterColumn("dbo.employee_leave", "Date", c => c.DateTime(nullable: false));
            AlterColumn("dbo.employee_leave", "Reason", c => c.String());
            AlterColumn("dbo.employee_loan", "LoanDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.employee_loan", "StartDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.employee_loan", "EndDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.employee_loan", "PaymentStartDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.employee_loan", "WeeklyPaymentDayOfWeek", c => c.DateTime(nullable: false));
            AlterColumn("dbo.employee_loan", "BiMonthlyPaymentFirstDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.employee_loan", "BiMonthlyPaymentSecondDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.employee_loan", "MonthlyPaymentDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.employee", "BirthDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.employee_workschedule", "DateCreated", c => c.DateTime(nullable: false));
            AlterColumn("dbo.holiday", "Date", c => c.DateTime(nullable: false));
            AlterColumn("dbo.leave", "Description", c => c.String());
            AlterColumn("dbo.loan_payment", "PaymentDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.payroll", "PayrollDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.payroll", "CutOffStartDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.payroll", "CutOffEndDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.payroll", "PayrollGeneratedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.positions", "Description", c => c.String());
            AlterColumn("dbo.schedule", "StartTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.schedule", "EndTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.settings", "SettingKey", c => c.String());
            AlterColumn("dbo.settings", "Value", c => c.String());
            AlterColumn("dbo.settings", "Description", c => c.String());
            AlterColumn("dbo.settings", "Category", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.settings", "Category", c => c.String(unicode: false));
            AlterColumn("dbo.settings", "Description", c => c.String(unicode: false));
            AlterColumn("dbo.settings", "Value", c => c.String(unicode: false));
            AlterColumn("dbo.settings", "SettingKey", c => c.String(unicode: false));
            AlterColumn("dbo.schedule", "EndTime", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.schedule", "StartTime", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.positions", "Description", c => c.String(unicode: false));
            AlterColumn("dbo.payroll", "PayrollGeneratedDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.payroll", "CutOffEndDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.payroll", "CutOffStartDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.payroll", "PayrollDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.loan_payment", "PaymentDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.leave", "Description", c => c.String(unicode: false));
            AlterColumn("dbo.holiday", "Date", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.employee_workschedule", "DateCreated", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.employee", "BirthDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.employee_loan", "MonthlyPaymentDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.employee_loan", "BiMonthlyPaymentSecondDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.employee_loan", "BiMonthlyPaymentFirstDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.employee_loan", "WeeklyPaymentDayOfWeek", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.employee_loan", "PaymentStartDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.employee_loan", "EndDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.employee_loan", "StartDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.employee_loan", "LoanDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.employee_leave", "Reason", c => c.String(unicode: false));
            AlterColumn("dbo.employee_leave", "Date", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.employee_adjustment", "Remarks", c => c.String(unicode: false));
            AlterColumn("dbo.employee_adjustment", "Date", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.deduction", "Remarks", c => c.String(unicode: false));
            AlterColumn("dbo.attendance", "ClockOut", c => c.DateTime(precision: 0));
            AlterColumn("dbo.attendance", "ClockIn", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("dbo.attendance_log", "ClockInOut", c => c.DateTime(nullable: false, precision: 0));
            DropColumn("dbo.employee", "Picture");
        }
    }
}
