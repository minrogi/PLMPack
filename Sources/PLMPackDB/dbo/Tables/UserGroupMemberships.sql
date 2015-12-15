CREATE TABLE [dbo].[UserGroupMemberships] (
    [UserId]  NVARCHAR (128) NOT NULL,
    [GroupId] NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_dbo.UserGroupMemberships] PRIMARY KEY CLUSTERED ([UserId] ASC, [GroupId] ASC),
    CONSTRAINT [FK_dbo.UserGroupMemberships_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.UserGroupMemberships_dbo.Groups_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Groups] ([Id]) ON DELETE CASCADE
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UserGroupMemberships]
    ON [dbo].[UserGroupMemberships]([UserId] ASC);

