CREATE TABLE `employee_hours_total_per_company` (
  `TotalEmployeeHoursId` int(11) NOT NULL,
  `TotalEmployeeHoursPerCompanyId` int(11) NOT NULL,
  `CompanyId` int(11) NOT NULL,
  PRIMARY KEY (`TotalEmployeeHoursId`),
  KEY `IX_TotalEmployeeHoursId` (`TotalEmployeeHoursId`) USING HASH,
  KEY `IX_CompanyId` (`CompanyId`) USING HASH,
  CONSTRAINT `FK_c0c44e91d0ff4fb381c91412f85c998b` FOREIGN KEY (`TotalEmployeeHoursId`) REFERENCES `employee_hours_total` (`TotalEmployeeHoursId`),
  CONSTRAINT `FK_employee_hours_total_per_company_company_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `company` (`CompanyId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
