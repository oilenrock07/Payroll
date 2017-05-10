CREATE TABLE `employee_payroll_per_company` (
  `PayrollId` int(11) NOT NULL,
  `EmployeePayrollPerCompanyId` int(11) NOT NULL,
  `CompanyId` int(11) NOT NULL,
  PRIMARY KEY (`PayrollId`),
  KEY `IX_PayrollId` (`PayrollId`) USING HASH,
  KEY `IX_CompanyId` (`CompanyId`) USING HASH,
  CONSTRAINT `FK_employee_payroll_per_company_company_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `company` (`CompanyId`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_employee_payroll_per_company_payroll_PayrollId` FOREIGN KEY (`PayrollId`) REFERENCES `payroll` (`PayrollId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
;