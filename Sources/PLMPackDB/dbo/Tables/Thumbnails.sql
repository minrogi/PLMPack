CREATE TABLE [dbo].[Thumbnails] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [FileGuid] NVARCHAR (128) NOT NULL,
    [Width]    INT            NOT NULL,
    [Height]   INT            NOT NULL,
    [MimeType] NVARCHAR (16)  NOT NULL,
    CONSTRAINT [PK_dbo.Thumbnails] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Thumbnails_dbo.File_Guid] FOREIGN KEY ([FileGuid]) REFERENCES [dbo].[Files] ([Guid])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [ThumbnailsIdIndex]
    ON [dbo].[Thumbnails]([Id] ASC);

