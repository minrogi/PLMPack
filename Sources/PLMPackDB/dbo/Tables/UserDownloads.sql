CREATE TABLE [dbo].[UserDownloads] (
    [Id]           INT            NOT NULL,
    [UserId]       NVARCHAR (128) NOT NULL,
    [DocumentId]   NVARCHAR (128) NOT NULL,
    [DateDownload] DATETIME       NOT NULL,
    [FileFormat]   NVARCHAR (5)   NULL,
    CONSTRAINT [PK_dbo.UserDownloads] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.UserDownloads_dbo.Documents_Id] FOREIGN KEY ([DocumentId]) REFERENCES [dbo].[Documents] ([Id]),
    CONSTRAINT [FK_dbo.UserDownloads_dbo.Users_Id] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);

