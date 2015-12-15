CREATE TABLE [dbo].[CardboardProfiles] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (128) NOT NULL,
    [Description] NVARCHAR (256) NULL,
    [Code]        NVARCHAR (8)   NOT NULL,
    [Thickness]   FLOAT (53)     NOT NULL,
    [GroupId]     NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_dbo.CardboardProfiles] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.CardboardProfiles_dbo.Groups_Id] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Groups] ([Id]) ON DELETE CASCADE
);

