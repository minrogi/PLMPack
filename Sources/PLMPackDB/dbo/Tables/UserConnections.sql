CREATE TABLE [dbo].[UserConnections] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [UserId]         NVARCHAR (128) NOT NULL,
    [DateConnectIN]  DATETIME       NOT NULL,
    [DateConnectOUT] DATETIME       NULL,
    CONSTRAINT [PK_dbo.UserConnections] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.UserConnections_dbo.AspNetUsers_Id] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);

