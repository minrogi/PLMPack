CREATE TABLE [dbo].[MajorationSets] (
	[Id] NVARCHAR (128) NOT NULL,
    [ComponentGuid]      NVARCHAR (128) NOT NULL,
    [CardboardProfileId] INT            NOT NULL,
    CONSTRAINT [PK_dbo.MajorationSets] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.MajorationSets_dbo.CardboardProfiles_Id] FOREIGN KEY ([CardboardProfileId]) REFERENCES [dbo].[CardboardProfiles] ([Id]),
    CONSTRAINT [FK_dbo.MajorationSets_dbo.Component_Guid] FOREIGN KEY ([ComponentGuid]) REFERENCES [dbo].[Components] ([Guid])
);

