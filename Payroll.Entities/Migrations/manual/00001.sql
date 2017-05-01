ALTER TABLE `payroll_frisco_prod`.`leave` 
ADD COLUMN `IsPayable` TINYINT(1) NULL DEFAULT 0 COMMENT '' AFTER `UpdateDate`,
ADD COLUMN `IsHolidayAfterPayable` TINYINT(1) NULL DEFAULT 0 COMMENT '' AFTER `IsPayable`;
