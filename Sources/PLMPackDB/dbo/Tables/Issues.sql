CREATE TABLE [dbo].[Issues] (
    [Id]         INT            NOT NULL,
    [UserNoteId] NVARCHAR (128) NOT NULL,
    [DocumentId] NVARCHAR (128) NULL,
    [Type]       INT            NOT NULL,
    [Status]     INT            NOT NULL,
    [Title]      NVARCHAR (256) NOT NULL,
    CONSTRAINT [PK_dbo.Issues] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Issues_dbo.Documents_Id] FOREIGN KEY ([DocumentId]) REFERENCES [dbo].[Documents] ([Id]),
    CONSTRAINT [FK_dbo.Issues_dbo.UserNotes_Id] FOREIGN KEY ([UserNoteId]) REFERENCES [dbo].[UserNotes] ([Id])
);

