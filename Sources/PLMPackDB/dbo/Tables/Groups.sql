CREATE TABLE [dbo].[Groups] (
    [Id]          NVARCHAR (128) NOT NULL,
    [GroupName]   NVARCHAR (256) NOT NULL,
    [GroupDesc]   NVARCHAR (256) NULL,
    [DateCreated] DATETIME       NOT NULL,
    [UserId]      NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_dbo.Groups] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Groups_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [GroupNameIndex]
    ON [dbo].[Groups]([GroupName] ASC);

