namespace Payroll.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test2 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "tbl_deduction", newName: "deduction");
            RenameTable(name: "tbl_employee_adjustment", newName: "employee_adjustment");
            RenameTable(name: "tbl_employee_leave", newName: "employee_leave");
            RenameTable(name: "tbl_employee_loan", newName: "employee_loan");
            RenameTable(name: "tbl_payroll", newName: "payroll");
        }
        
        public override void Down()
        {
            RenameTable(name: "payroll", newName: "tbl_payroll");
            RenameTable(name: "employee_loan", newName: "tbl_employee_loan");
            RenameTable(name: "employee_leave", newName: "tbl_employee_leave");
            RenameTable(name: "employee_adjustment", newName: "tbl_employee_adjustment");
            RenameTable(name: "deduction", newName: "tbl_deduction");
        }
    }
}
