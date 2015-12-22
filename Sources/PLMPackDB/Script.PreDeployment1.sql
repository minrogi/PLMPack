/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
USE PLMPackDB
GO
IF OBJECT_ID('dbo.__MigrationHistory', 'U') IS NOT NULL
	DROP TABLE dbo.__MigrationHistory;
	/*DELETE FROM [__MigrationHistory]*/
GO
IF OBJECT_ID('dbo.TreeNodeGroupShares', 'U') IS NOT NULL
	DROP TABLE dbo.TreeNodeGroupShares;
	/*DELETE FROM [TreeNodeGroupShares]*/
GO
IF OBJECT_ID('dbo.TreeNodeDocuments', 'U') IS NOT NULL
	DROP TABLE dbo.TreeNodeDocuments;
	/*DELETE FROM [TreeNodeDocuments]*/
GO
IF OBJECT_ID('dbo.TreeNodes', 'U') IS NOT NULL
	DROP TABLE dbo.TreeNodes;
	/*DELETE FROM [TreeNodes]*/
GO
IF OBJECT_ID('dbo.Majorations', 'U') IS NOT NULL
	DROP TABLE dbo.Majorations;
	/*DELETE FROM [Majorations]*/
GO
IF OBJECT_ID('dbo.MajorationSets', 'U') IS NOT NULL
	DROP TABLE dbo.MajorationSets;
	/*DELETE FROM [MajorationSets]*/
GO
IF OBJECT_ID('dbo.ParamDefaultComponents', 'U') IS NOT NULL
	DROP TABLE dbo.ParamDefaultComponents;
	/*DELETE FROM [ParamDefaultComponents]*/
GO
IF OBJECT_ID('dbo.Components', 'U') IS NOT NULL
	DROP TABLE dbo.Components;
	/*DELETE FROM [Components]*/
GO
IF OBJECT_ID('dbo.UserDownloads', 'U') IS NOT NULL
	DROP TABLE dbo.UserDownloads;
	/*DELETE FROM [UserDownloads]*/
GO
IF OBJECT_ID('dbo.Issues', 'U') IS NOT NULL
	DROP TABLE dbo.Issues;
	/*DELETE FROM [Issues]*/
GO
IF OBJECT_ID('dbo.Documents', 'U') IS NOT NULL
	DROP TABLE dbo.Documents;
	/*DELETE FROM [Documents]*/
GO
IF OBJECT_ID('dbo.Thumbnails', 'U') IS NOT NULL
	DROP TABLE dbo.Thumbnails;
	/*DELETE FROM [Thumbnails]*/
GO
IF OBJECT_ID('dbo.Files', 'U') IS NOT NULL
	DROP TABLE dbo.Files;
	/*DELETE FROM [Files]*/
GO
IF OBJECT_ID('dbo.CardboardQuality', 'U') IS NOT NULL
	DROP TABLE dbo.CardboardQuality;
	/*DELETE FROM [CardboardQuality]*/
GO
IF OBJECT_ID('dbo.CardboardProfiles', 'U') IS NOT NULL
	DROP TABLE dbo.CardboardProfiles;
	/*DELETE FROM [CardboardProfiles]*/
GO
IF OBJECT_ID('dbo.CardboardFormat', 'U') IS NOT NULL
	DROP TABLE dbo.CardboardFormat;
	/*DELETE FROM [CardboardFormat]*/
GO
IF OBJECT_ID('dbo.GroupsOfInterest', 'U') IS NOT NULL
	DROP TABLE dbo.GroupsOfInterest;
	/*DELETE FROM [GroupsOfInterest]*/
GO
IF OBJECT_ID('dbo.UserGroupMemberships', 'U') IS NOT NULL
	DROP TABLE dbo.UserGroupMemberships;
	/*DELETE FROM [UserGroupMemberships]*/
GO
IF OBJECT_ID('dbo.Groups', 'U') IS NOT NULL
	DROP TABLE dbo.Groups;
	/*DELETE FROM [Groups]*/
GO
IF OBJECT_ID('dbo.UserNotes', 'U') IS NOT NULL
	DROP TABLE dbo.UserNotes;
	/*DELETE FROM [UserNotes]*/
GO
IF OBJECT_ID('dbo.UserConnections', 'U') IS NOT NULL
	DROP TABLE dbo.UserConnections;
	/*DELETE FROM [UserConnections]*/
GO
IF OBJECT_ID('dbo.AspNetUserClaims', 'U') IS NOT NULL
	DROP TABLE dbo.AspNetUserClaims;
	/*DELETE FROM [AspNetUserClaims]*/
GO
IF OBJECT_ID('dbo.AspNetUserLogins', 'U') IS NOT NULL
	DROP TABLE dbo.AspNetUserLogins;
	/*DELETE FROM [AspNetUserLogins]*/
GO
IF OBJECT_ID('dbo.AspNetUserRoles', 'U') IS NOT NULL
	DROP TABLE dbo.AspNetUserRoles;
	/*DELETE FROM [AspNetUserRoles]*/
GO
IF OBJECT_ID('dbo.AspNetRoles', 'U') IS NOT NULL
	DROP TABLE dbo.AspNetRoles;
	/*DELETE FROM [AspNetRoles]*/
GO
IF OBJECT_ID('dbo.AspNetUsers', 'U') IS NOT NULL
	DROP TABLE dbo.AspNetUsers;
	/*DELETE FROM [dbo.AspNetUsers];*/
GO
