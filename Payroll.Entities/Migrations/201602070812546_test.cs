namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.attendance",
                c => new
                    {
                        AttendanceId = c.Int(nullable: false, identity: true),
                        EmployeeCode = c.String(maxLength: 50, storeType: "nvarchar"),
                        ClockIn = c.DateTime(nullable: false, precision: 0),
                        ClockOut = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => t.AttendanceId);
            
            CreateTable(
                "dbo.tbl_deduction",
                c => new
                    {
                        DeductionId = c.Int(nullable: false, identity: true),
                        DeductionName = c.String(maxLength: 50, storeType: "nvarchar"),
                        Remarks = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.DeductionId);
            
            CreateTable(
                "dbo.department_manager",
                c => new
                    {
                        DepartmentManagerId = c.Int(nullable: false, identity: true),
                        DepartmentId = c.Int(nullable: false),
                        ManagerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DepartmentManagerId);
            
            CreateTable(
                "dbo.department",
                c => new
                    {
                        DepartmentId = c.Int(nullable: false, identity: true),
                        DepartmentName = c.String(maxLength: 250, storeType: "nvarchar"),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DepartmentId);
            
            CreateTable(
                "dbo.tbl_employee_adjustment",
                c => new
                    {
                        AdjustmentId = c.Int(nullable: false, identity: true),
                        AdjustmentTypeId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false, precision: 0),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remarks = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.AdjustmentId);
            
            CreateTable(
                "dbo.employee_department",
                c => new
                    {
                        EmployeeDepartmentId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeDepartmentId);
            
            CreateTable(
                "dbo.employee_files",
                c => new
                    {
                        EmployeeFileId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        FileId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeFileId);
            
            CreateTable(
                "dbo.employee_info",
                c => new
                    {
                        EmploymentInfoId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        PaymentFrequencyId = c.Int(nullable: false),
                        PositionId = c.Int(nullable: false),
                        Salary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Allowance = c.Decimal(precision: 18, scale: 2),
                        TIN = c.String(maxLength: 50, storeType: "nvarchar"),
                        SSS = c.String(maxLength: 50, storeType: "nvarchar"),
                        GSIS = c.String(maxLength: 50, storeType: "nvarchar"),
                        PAGIBIG = c.String(maxLength: 50, storeType: "nvarchar"),
                        PhilHealth = c.String(maxLength: 50, storeType: "nvarchar"),
                        Dependents = c.Int(nullable: false),
                        Married = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.EmploymentInfoId);
            
            CreateTable(
                "dbo.tbl_employee_leave",
                c => new
                    {
                        EmployeeLeaveId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        LeaveId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false, precision: 0),
                        Reason = c.String(unicode: false),
                        IsApproved = c.Boolean(nullable: false),
                        ApprovedBy = c.Int(nullable: false),
                        Hours = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeLeaveId);
            
            CreateTable(
                "dbo.tbl_employee_loan",
                c => new
                    {
                        EmployeeLoanId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        LoanId = c.Int(nullable: false),
                        FrequencyId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LoanDate = c.DateTime(nullable: false, precision: 0),
                        StartDate = c.DateTime(nullable: false, precision: 0),
                        EndDate = c.DateTime(nullable: false, precision: 0),
                        IsActive = c.Boolean(nullable: false),
                        PaymentStartDate = c.DateTime(nullable: false, precision: 0),
                        WeeklyPaymentDayOfWeek = c.DateTime(nullable: false, precision: 0),
                        BiMonthlyPaymentFirstDate = c.DateTime(nullable: false, precision: 0),
                        BiMonthlyPaymentSecondDate = c.DateTime(nullable: false, precision: 0),
                        MonthlyPaymentDate = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.EmployeeLoanId);
            
            CreateTable(
                "dbo.employee",
                c => new
                    {
                        EmployeeId = c.Int(nullable: false, identity: true),
                        EmployeeCode = c.String(maxLength: 250, storeType: "nvarchar"),
                        FirstName = c.String(maxLength: 50, storeType: "nvarchar"),
                        LastName = c.String(maxLength: 50, storeType: "nvarchar"),
                        MiddleName = c.String(maxLength: 50, storeType: "nvarchar"),
                        BirthDate = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.EmployeeId);
            
            CreateTable(
                "dbo.employee_workschedule",
                c => new
                    {
                        EmployeeWorkScheduleId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        WorkScheduleId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.EmployeeWorkScheduleId);
            
            CreateTable(
                "dbo.files",
                c => new
                    {
                        FileId = c.Int(nullable: false, identity: true),
                        FileName = c.String(maxLength: 250, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.FileId);
            
            CreateTable(
                "dbo.frequency",
                c => new
                    {
                        FrequencyId = c.Int(nullable: false, identity: true),
                        FrequencyName = c.String(maxLength: 50, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.FrequencyId);
            
            CreateTable(
                "dbo.holiday",
                c => new
                    {
                        HolidayId = c.Int(nullable: false, identity: true),
                        HolidayName = c.String(maxLength: 50, storeType: "nvarchar"),
                        IsRegularHoliday = c.Boolean(nullable: false),
                        Date = c.DateTime(nullable: false, precision: 0),
                        IsActive = c.Boolean(nullable: false),
                        Year = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.HolidayId);
            
            CreateTable(
                "dbo.leave",
                c => new
                    {
                        LeaveId = c.Int(nullable: false, identity: true),
                        LeaveName = c.String(maxLength: 250, storeType: "nvarchar"),
                        IsActive = c.Boolean(nullable: false),
                        IsRefundable = c.Boolean(nullable: false),
                        Count = c.Int(nullable: false),
                        Description = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.LeaveId);
            
            CreateTable(
                "dbo.loan_payment",
                c => new
                    {
                        LoanPaymentId = c.Int(nullable: false, identity: true),
                        EmployeeLoanId = c.Int(nullable: false),
                        PaymentDate = c.DateTime(nullable: false, precision: 0),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.LoanPaymentId);
            
            CreateTable(
                "dbo.loan",
                c => new
                    {
                        LoanId = c.Int(nullable: false, identity: true),
                        LoanName = c.String(maxLength: 50, storeType: "nvarchar"),
                        Min = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Max = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Interest = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.LoanId);
            
            CreateTable(
                "dbo.payment_frequency",
                c => new
                    {
                        PaymentFrequencyId = c.Int(nullable: false, identity: true),
                        FrequencyId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        WeeklyStartDayOfWeek = c.Int(),
                        MonthlyStartDay = c.Int(),
                        MonthlyEndDay = c.Int(),
                    })
                .PrimaryKey(t => t.PaymentFrequencyId);
            
            CreateTable(
                "dbo.tbl_payroll",
                c => new
                    {
                        PayrollId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        Salary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDeduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAdjustment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PayrollDate = c.DateTime(nullable: false, precision: 0),
                        CutOffStartDate = c.DateTime(nullable: false, precision: 0),
                        CutOffEndDate = c.DateTime(nullable: false, precision: 0),
                        PayrollGeneratedDate = c.DateTime(nullable: false, precision: 0),
                        TotalPay = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.PayrollId);
            
            CreateTable(
                "dbo.positions",
                c => new
                    {
                        PositionId = c.Int(nullable: false, identity: true),
                        PositionName = c.String(maxLength: 150, storeType: "nvarchar"),
                        Description = c.String(unicode: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.PositionId);
            
            CreateTable(
                "dbo.schedule",
                c => new
                    {
                        ScheduleId = c.Int(nullable: false, identity: true),
                        StartDay = c.Int(nullable: false),
                        EndDay = c.Int(nullable: false),
                        Timeperiod = c.Int(nullable: false),
                        StartTime = c.DateTime(nullable: false, precision: 0),
                        EndTime = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.ScheduleId);
            
            CreateTable(
                "dbo.settings",
                c => new
                    {
                        SettingId = c.Int(nullable: false, identity: true),
                        SettingKey = c.String(unicode: false),
                        Value = c.String(unicode: false),
                        Description = c.String(unicode: false),
                        Category = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.SettingId);
            
            CreateTable(
                "dbo.tax",
                c => new
                    {
                        TaxId = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 50, storeType: "nvarchar"),
                        Frequency = c.Int(nullable: false),
                        NoOfDependents = c.Int(nullable: false),
                        BaseAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverPercentage = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TaxId);
            
            CreateTable(
                "dbo.work_schedule",
                c => new
                    {
                        WorkScheduleId = c.Int(nullable: false, identity: true),
                        WorkScheduleName = c.String(maxLength: 250, storeType: "nvarchar"),
                        TimeStart = c.Int(nullable: false),
                        TimeEnd = c.Int(nullable: false),
                        WeekStart = c.Int(nullable: false),
                        WeekEnd = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WorkScheduleId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.work_schedule");
            DropTable("dbo.tax");
            DropTable("dbo.settings");
            DropTable("dbo.schedule");
            DropTable("dbo.positions");
            DropTable("dbo.tbl_payroll");
            DropTable("dbo.payment_frequency");
            DropTable("dbo.loan");
            DropTable("dbo.loan_payment");
            DropTable("dbo.leave");
            DropTable("dbo.holiday");
            DropTable("dbo.frequency");
            DropTable("dbo.files");
            DropTable("dbo.employee_workschedule");
            DropTable("dbo.employee");
            DropTable("dbo.tbl_employee_loan");
            DropTable("dbo.tbl_employee_leave");
            DropTable("dbo.employee_info");
            DropTable("dbo.employee_files");
            DropTable("dbo.employee_department");
            DropTable("dbo.tbl_employee_adjustment");
            DropTable("dbo.department");
            DropTable("dbo.department_manager");
            DropTable("dbo.tbl_deduction");
            DropTable("dbo.attendance");
        }
    }
}
