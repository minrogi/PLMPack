CREATE TABLE [dbo].[Documents] (
    [Id]           NVARCHAR (128) NOT NULL,
    [Name]         NVARCHAR (128) NOT NULL,
    [Description]  NVARCHAR (256) NULL,
    [DocumentType] NVARCHAR (32)  NOT NULL,
    [FileGuid]     NVARCHAR (128) NOT NULL,
    [GroupId]      NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_dbo.Documents] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Documents_dbo.File_Guid] FOREIGN KEY ([FileGuid]) REFERENCES [dbo].[Files] ([Guid]),
    CONSTRAINT [FK_dbo.Documents_dbo.Groups_Id] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Groups] ([Id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [DocumentsIdIndex]
    ON [dbo].[Documents]([Id] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [MajorationSetsIdIndex]
    ON [dbo].[Documents]([Id] ASC);

