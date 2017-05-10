CREATE TABLE `employee_payroll_item_per_company` (
  `EmployeePayrollItemId` int(11) NOT NULL,
  `EmployeePayrollItemPerCompanyId` int(11) NOT NULL,
  `CompanyId` int(11) NOT NULL,
  PRIMARY KEY (`EmployeePayrollItemId`),
  KEY `IX_EmployeePayrollItemId` (`EmployeePayrollItemId`) USING HASH,
  KEY `IX_CompanyId` (`CompanyId`) USING HASH,
  CONSTRAINT `FK_846ccd73cc814329a61cd2887db8d3d1` FOREIGN KEY (`EmployeePayrollItemId`) REFERENCES `employee_payroll_item` (`EmployeePayrollItemId`),
  CONSTRAINT `FK_employee_payroll_item_per_company_company_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `company` (`CompanyId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
