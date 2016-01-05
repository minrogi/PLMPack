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
  (N'0890653a-b1d7-4b66-86d5-591a90fc92b1', N'fgasnier71@gmail.com', 'True', N'ADLRtj8jbq8fgJkXBLrLV+YgcEhFUanRPqWR2PzxHU23t1pHO04Gas8ALrcvSF4GiA==',N'a6dd8a18-cf73-41d3-9c20-1ec9d2819d95', NULL, 'False', 'False', NULL, 'False', 0, N'fgasnier71', N'125ee332-f8ec-462a-a925-39070db95e49')
) 
AS Source (Id, Email, EmailConfirmed, PasswordHash, SecurityStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEndDateUtc, LockoutEnabled, AccessFailedCount, UserName, GroupId)
ON Target.Id = Source.Id OR Target.UserName = Source.UserName
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

MERGE INTO UserGroupMemberships AS Target
USING (VALUES
	(N'0890653a-b1d7-4b66-86d5-591a90fc92b1', N'2a1c6794-c753-4665-a002-3c2a8b99ad72'),
	(N'0890653a-b1d7-4b66-86d5-591a90fc92b1', N'125ee332-f8ec-462a-a925-39070db95e49')
)
AS Source (UserId, GroupId)
ON Target.UserId = Source.UserId AND Target.GroupId = Source.GroupId
WHEN MATCHED THEN
UPDATE SET GroupId = Source.GroupId   
-- update matched rows
WHEN NOT MATCHED BY TARGET THEN
INSERT (UserId, GroupId)
VALUES (UserId, GroupId)
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

MERGE INTO Files As Target
USING (VALUES
	-- TREEDIM
	(N'3BD67B77-6BB4-4A5E-AD49-46F686CA6340', N'png', CAST('2016-01-01 12:00:00 AM' AS DATETIME2)),
	-- FOLDER
	(N'affbf3ec-cca4-4ebe-87c7-03960a7134d6', N'png', CAST('2016-01-01 12:00:00 AM' AS DATETIME2))


	-- 
)
AS Source(Guid, Extension, DateCreated)
ON Target.Guid=Source.Guid
-- update matched rows
WHEN MATCHED THEN
UPDATE SET Extension = Source.Extension
-- insert new row
WHEN NOT MATCHED BY TARGET THEN
INSERT (Guid, Extension, DateCreated)
VALUES (Guid, Extension, DateCreated)
;

MERGE INTO Thumbnails As Target
USING (VALUES
	--TREEDIM
	(N'3BD67B77-6BB4-4A5E-AD49-46F686CA6340', 150, 150, N'image/png'),
	--FOLDER
	(N'affbf3ec-cca4-4ebe-87c7-03960a7134d6', 150, 150, N'image/png')
)
AS Source(FileGuid, Width, Height, MimeType)
ON Target.FileGuid=Source.FileGuid
-- update matched rows
WHEN MATCHED THEN
UPDATE SET MimeType = Source.MimeType
-- insert new row
WHEN NOT MATCHED BY TARGET THEN
INSERT (FileGuid, Width, Height, MimeType)
VALUES (FileGuid, Width, Height, MimeType)
;

MERGE INTO TreeNodes As Target
USING (VALUES
	(N'072D09C6-023C-45A7-8111-226BF1FD9218', N'treeDiM', N'treeDiM group root node', 1, NULL, N'125ee332-f8ec-462a-a925-39070db95e49')
)
AS Source(Id, Name, Description, ThumbnailId, ParentNodeId,GroupId)
ON Target.Id = Source.Id
-- update matched rows
WHEN MATCHED THEN
UPDATE SET Name=Source.Name
-- insert new row
WHEN NOT MATCHED BY TARGET THEN
INSERT (Id, Name, Description, ThumbnailId, ParentNodeId, GroupId)
VALUES (Id, Name, Description, ThumbnailId, ParentNodeId, GroupId)
;
 