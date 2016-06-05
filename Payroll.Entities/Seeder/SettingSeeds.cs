    using System.Collections.Generic;

namespace Payroll.Entities.Seeder
{
    public class SettingSeeds : ISeeders<Setting>
    {
        public IEnumerable<Setting> GetDefaultSeeds()
        {
            return new List<Setting>()
            {
                //Payroll Settings
                new Setting { SettingKey = "SCHEDULE_NIGHTDIF_TIME_START", Value= "10:00:00 PM", Description="Night Differential Start Time", Category= "SCHEDULE" },
                new Setting { SettingKey = "SCHEDULE_NIGHTDIF_TIME_END", Value = "7:59:00 AM", Description = "Night Differential End Time", Category = "SCHEDULE" },
                new Setting { SettingKey = "SCHEDULE_ADVANCE_OT_PERIOD_MINUTES", Value = "15", Description = "Advance OT Period in minutes", Category = "SCHEDULE" },
                new Setting { SettingKey = "SCHEDULE_GRACE_PERIOD_MINUTES", Value = "5", Description = "Grace period in minutes", Category = "SCHEDULE" },
                new Setting { SettingKey = "SCHEDULE_MINIMUM_OT_MINUTES", Value = "5", Description = "Minimum OT in minutes", Category = "SCHEDULE" },

                new Setting { SettingKey = "RATE_OT", Value = "0.25", Description = "OT Rate", Category = "RATE" },
                new Setting { SettingKey = "RATE_NIGHTDIF", Value = "0.8", Description = "Night Dif", Category = "RATE" },
                new Setting { SettingKey = "RATE_REST_DAY", Value = "0.3", Description = "Rest Day", Category = "RATE" },
                new Setting { SettingKey = "RATE_HOLIDAY_SPECIAL", Value = "0.3", Description = "Special Holiday", Category = "RATE" },
                new Setting { SettingKey = "RATE_HOLIDAY_REGULAR", Value = "1", Description = "Regular Holiday", Category = "RATE" },
                new Setting { SettingKey = "RATE_OT_HOLIDAY", Value = "0.3", Description = "OT Rate Holiday", Category = "RATE" },

                //Company Info
                new Setting { SettingKey = "COMPANY_NAME", Value = "Lychee Co", Description = "Company Name", Category = "COMPANY_INFO" },
                new Setting { SettingKey = "COMPANY_TYPE", Value = "Private", Description = "Company Is Private or Public? For SSS OR GSIS Field", Category = "COMPANY_INFO" },

                //Company Settings
                new Setting { SettingKey = "SUPPORT_REFUNDABLE_LEAVE", Value = "true", Description = "Whether or not, the company supports refundable leaves", Category = "COMPANY_SETTING" },
                new Setting { SettingKey = "HOLIDAY_CURRENT_YEAR", Value = "2016", Description = "This will be changed every year. This value will only be used in Payroll.Scheduler", Category = "COMPANY_SETTING" },
                new Setting { SettingKey = "IS_PRIVATE_COMPANY", Value = "true", Description = "Will Determine if SSS or GSIS", Category = "COMPANY_SETTING" },
                
                //Application Settings
                new Setting { SettingKey = "EMPLOYEE_IMAGE_PATH", Value = "~/Images/Employee/", Description = "Relative image path of the employee's pictures", Category = "APP" },
                new Setting { SettingKey = "ALLOW_EXTERNAL_LOGIN", Value = "false", Description = "Should allow the app to have external login e.g. facebook, twitter, gmail", Category = "APP" },
                new Setting { SettingKey = "PAGINATION_ITEMS_PER_PAGE", Value = "30", Description = "Conrtrols the item count to display within the pagination", Category = "APP" },

                //for login display
                new Setting { SettingKey = "DISPLAY_LOGIN_URL", Value = "http://payroll.logindisplay/api/payrollapi/", Description = "Route of display picture upon login", Category = "URL" },

                /* Settings for PAYROLL */
                //Settings of payroll
                new Setting { SettingKey = "PAYROLL_FREQUENCY", Value = "3", Description = "Payroll Frequency", Category = "PAYROLL_SCHEDULE"},
                new Setting { SettingKey = "PAYROLL_WEEK_START", Value = "3", Description = "Payroll Week Start", Category = "PAYROLL_SCHEDULE"},
                new Setting { SettingKey = "PAYROLL_WEEK_END", Value = "2", Description = "Payroll Week End", Category = "PAYROLL_SCHEDULE"},
                new Setting { SettingKey = "PAYROLL_REGULAR_HOURS", Value = "8", Description = "Payroll Total Regular Hours", Category = "PAYROLL_SCHEDULE"},
                new Setting { SettingKey = "PAYROLL_TOTAL_HOURS", Value = "10", Description = "Payroll Total Hours", Category = "PAYROLL_SCHEDULE"},
                new Setting { SettingKey = "PAYROLL_IS_SPHOLIDAY_WITH_PAY", Value = "0", Description = "Is Specia holiday with pay", Category = "PAYROLL_SCHEDULE"},
                
                //Adjustments
                new Setting { SettingKey = "ALLOWANCE_TOTAL_DAYS", Value = "26", Description = "Allowance total number of working days monthly", Category = "PAYROLL_ADJUSTMENTS"},
                new Setting { SettingKey = "ALLOWANCE_WEEK_SCHEDULE", Value = "1", Description = "Allowance week schedule", Category = "PAYROLL_ADJUSTMENTS"},
                new Setting { SettingKey = "ALLOWANCE_DAY_SCHEDULE", Value = "6", Description = "Allowance week schedule", Category = "PAYROLL_ADJUSTMENTS"},

                //settings for deduction
                new Setting { SettingKey = "HDMF_MAX_MONTHLY_COMPENSATION", Value = "5000", Description = "HDMF Maximum Monthly Compensation Contributions", Category = "PAYROLL_DEDUCTIONS"},
                
                //Schedule of deductions
                new Setting { SettingKey = "DEDUCTION_IS_SEMIMONTHLY", Value = "0", Description = "If deduction computation is semi monthly or monthly. 1 if semimonthly, 0 if monthly", Category = "PAYROLL_DEDUCTIONS"},
                new Setting { SettingKey = "DEDUCTION_SEMIMONTHLY_SCHEDULE_1", Value = "15", Description = "Date of first deduction", Category = "PAYROLL_DEDUCTIONS"},
                new Setting { SettingKey = "DEDUCTION_SEMIMONTHLY_SCHEDULE_2", Value = "30", Description = "Date of second deduction", Category = "PAYROLL_DEDUCTIONS"},
                new Setting { SettingKey = "DEDUCTION_MONTHLY_SCHEDULE", Value = "30", Description = "Date of deduction", Category = "PAYROLL_DEDUCTIONS"},

                new Setting { SettingKey = "DEDUCTION_MONTHLY_TOTAL_HOURS", Value = "184", Description = "Number of hours if monthly", Category = "PAYROLL_DEDUCTIONS"},
                new Setting { SettingKey = "DEDUCTION_SEMIMONTHLY_TOTAL_HOURS", Value = "92", Description = "Number of hours if semimonthly", Category = "PAYROLL_DEDUCTIONS"},

                new Setting { SettingKey = "TAX_FREQUENCY", Value = "6", Description = "Computation of tax frequency", Category="TAX"}
            };
        }
    }
}
