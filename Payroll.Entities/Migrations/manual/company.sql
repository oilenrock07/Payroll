CREATE TABLE `frisco`.`company` (
  `CompanyId` INT NOT NULL,
  `CompanyCode` VARCHAR(20) NULL,
  `Address` VARCHAR(250) NULL,
  `CompanyInfo` VARCHAR(250) NULL,
  `IsActive` TINYINT(1) NULL,
  `CreateDate` DATETIME NULL,
  `UpdateDate` DATETIME NULL,
  PRIMARY KEY (`CompanyId`));
