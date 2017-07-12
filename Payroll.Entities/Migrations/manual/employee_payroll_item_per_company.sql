CREATE TABLE `employee_payroll_item_per_company` (
  `EmployeePayrollItemPerCompanyId` int(11) NOT NULL,
  `CompanyId` int(11) DEFAULT NULL,
  `EmployeePayrollItemId` int(11) DEFAULT NULL,
  `PayrollPerCompanyId` int(11) DEFAULT NULL,
  `PayrollDate` date DEFAULT NULL,
  `RateType` int(11) DEFAULT NULL,
  `EmployeeId` int(11) DEFAULT NULL,
  `TotalHours` double DEFAULT NULL,
  `Multiplier` double DEFAULT NULL,
  `TotalAmount` decimal(18,2) DEFAULT NULL,
  `RatePerHour` decimal(18,2) DEFAULT NULL,
  `IsActive` tinyint(1) DEFAULT NULL,
  `CreateDate` datetime DEFAULT NULL,
  `UpdateDate` datetime DEFAULT NULL,
  PRIMARY KEY (`EmployeePayrollItemPerCompanyId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
