CREATE TABLE [dbo].[TreeNodes] (
    [Id]           NVARCHAR (128) NOT NULL,
    [Name]         NVARCHAR (128) NOT NULL,
    [Description]  NVARCHAR (256) NULL,
    [ThumbnailId]  INT            NOT NULL,
    [ParentNodeId] NVARCHAR (128) NULL,
    [GroupId]      NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_dbo.TreeNodes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.TreeNodes_dbo.Groups_Id] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Groups] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.TreeNodes_dbo.Thumbnails_Id] FOREIGN KEY ([ThumbnailId]) REFERENCES [dbo].[Thumbnails] ([Id]) ON DELETE CASCADE
);
GO
CREATE UNIQUE NONCLUSTERED INDEX [TreeNodesIdIndex]
    ON [dbo].[TreeNodes]([Id] ASC);

