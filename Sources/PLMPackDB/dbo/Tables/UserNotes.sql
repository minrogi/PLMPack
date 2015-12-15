CREATE TABLE [dbo].[UserNotes] (
    [Id]           NVARCHAR (128)  NOT NULL,
    [UserId]       NVARCHAR (128)  NOT NULL,
    [ParentNoteId] NVARCHAR (128)  NULL,
    [DateCreated]  DATETIME        NOT NULL,
    [Text]         NVARCHAR (1024) NOT NULL,
    CONSTRAINT [PK_dbo.UserNotes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.UserNotes_dbo.AspNetUsers_Id] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_dbo.UserNotes_dbo.UserNotes_Id] FOREIGN KEY ([ParentNoteId]) REFERENCES [dbo].[UserNotes] ([Id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UserNoteIdIndex]
    ON [dbo].[UserNotes]([Id] ASC);

