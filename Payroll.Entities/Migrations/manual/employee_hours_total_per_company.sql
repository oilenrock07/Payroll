CREATE TABLE `frisco`.`employee_hours_total_per_company` (
  `TotalEmployeeHoursPerCompanyId` INT NOT NULL,
  `CompanyId` INT NULL,
  `TotalEmployeeHoursId` INT NULL,
  `EmployeeId` INT NULL,
  `Date` DATETIME NULL,
  `Hours` DOUBLE NULL,
  `Type` INT NULL,
  `IsActive` TINYINT(1) NULL,
  `CreateDate` DATETIME NULL,
  `UpdateDate` DATETIME NULL,
  PRIMARY KEY (`TotalEmployeeHoursPerCompanyId`));
