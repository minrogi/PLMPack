/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
-- Reference Data for AddressType 
MERGE INTO AspNetUsers AS Target 
USING (VALUES 
  (N'c05A12ca-e59a-4248-86b2-950851480c3d', N'admin@treedim.com', 'True', N'0==', N'c05A12ca-e59a-4248-86b2-950851480c3d', NULL, 'False', 'False', NULL, 'False', 0, N'admin', N'2a1c6794-c753-4665-a002-3c2a8b99ad72'),
  (N'0890653a-b1d7-4b66-86d5-591a90fc92b1', N'fgasnier71@gmail.com', 'True', N'AOb/XYW3CboViMJlIcipQiTcHVK9Q9qpTl/H6DczU6vxB2t7lxYBDoE4Eo1UlDTIVA==)',N'da710d75-5930-4929-a06d-a3c081476b84', NULL, 'False', 'False', NULL, 'False', 0, N'fgasnier71', N'125ee332-f8ec-462a-a925-39070db95e49')
) 
AS Source (Id, Email, EmailConfirmed, PasswordHash, SecurityStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEndDateUtc, LockoutEnabled, AccessFailedCount, UserName, GroupId)
ON Target.Id = Source.Id 
-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET UserName = Source.UserName 
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (Id, Email, EmailConfirmed, PasswordHash, SecurityStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEndDateUtc, LockoutEnabled, AccessFailedCount, UserName, GroupId) 
VALUES (Id, Email, EmailConfirmed, PasswordHash, SecurityStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEndDateUtc, LockoutEnabled, AccessFailedCount, UserName, GroupId)
; 

MERGE INTO Groups AS Target
USING (VALUES
	(N'2a1c6794-c753-4665-a002-3c2a8b99ad72', N'Everyone', N'Every user belongs to group Everyone', CAST('2016-01-01 12:00:00 AM' AS DATETIME2), N'c05A12ca-e59a-4248-86b2-950851480c3d'),
	(N'125ee332-f8ec-462a-a925-39070db95e49', N'treeDiM', N'treeDiM group', CAST('2016-01-01 12:00:00 AM' AS DATETIME2), N'0890653a-b1d7-4b66-86d5-591a90fc92b1')
)
AS Source(Id, GroupName, GroupDesc, DateCreated, UserId)
ON Target.Id = Source.Id
-- update matched rows
WHEN MATCHED THEN
UPDATE SET GroupName = Source.GroupName
-- insert new row
WHEN NOT MATCHED BY TARGET THEN
INSERT (Id, GroupName, GroupDesc, DateCreated, UserId)
VALUES (Id, GroupName, GroupDesc, DateCreated, UserId)
;

MERGE INTO GroupsOfInterest AS Target
USING (VALUES
	(N'0890653a-b1d7-4b66-86d5-591a90fc92b1', N'125ee332-f8ec-462a-a925-39070db95e49')
)
AS Source(UserId, GroupId)
ON Target.UserId=Source.UserId
-- update matched rows
WHEN MATCHED THEN
UPDATE SET GroupId = Source.GroupId
-- insert new row
WHEN NOT MATCHED BY TARGET THEN
INSERT (UserId, GroupId)
VALUES (UserId, GroupId)
;
