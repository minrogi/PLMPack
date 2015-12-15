CREATE TABLE [dbo].[CardboardFormat] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (128) NOT NULL,
    [Description] NVARCHAR (256) NULL,
    [Length]      FLOAT (53)     NOT NULL,
    [Width]       FLOAT (53)     NOT NULL,
    [GroupId]     NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_dbo.CardboardFormat] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.CardboardFormat_dbo.Groups_Id] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Groups] ([Id]) ON DELETE CASCADE
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [CardboardFormatIndex]
    ON [dbo].[CardboardFormat]([Id] ASC);

