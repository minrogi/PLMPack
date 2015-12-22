CREATE TABLE [dbo].[CardboardQuality] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [CardboardProfileId] INT            NOT NULL,
    [Name]               NVARCHAR (128) NOT NULL,
    [Description]        NVARCHAR (256) NULL,
    [SurfacicMass]       FLOAT (53)     NOT NULL,
    [RigidityX]          FLOAT (53)     NOT NULL,
    [RigidityY]          FLOAT (53)     NOT NULL,
    [YoungModulus]       FLOAT (53)     NOT NULL,
    [ECT]                FLOAT (53)     NOT NULL,
    CONSTRAINT [PK_dbo.CardboardQuality] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.CardboardQuality_dbo.CardboardProfiles_Id] FOREIGN KEY ([CardboardProfileId]) REFERENCES [dbo].[CardboardProfiles] ([Id]) ON DELETE CASCADE
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [CardboardQualityIdIndex]
    ON [dbo].[CardboardQuality]([Id] ASC);

