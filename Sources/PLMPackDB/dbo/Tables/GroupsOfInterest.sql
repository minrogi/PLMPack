CREATE TABLE [dbo].[GroupsOfInterest] (
    [UserId]  NVARCHAR (128) NOT NULL,
    [GroupId] NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_dbo.GroupsOfInterest] PRIMARY KEY NONCLUSTERED ([UserId] ASC, [GroupId] ASC),
    CONSTRAINT [FK_dbo.GroupsOfInterest_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_dbo.GroupsOfInterest_dbo.Groups_Id] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Groups] ([Id])
);
GO
CREATE CLUSTERED INDEX IX_GROUPSOFINTEREST ON GroupsOfInterest
(UserId Asc, GroupId Asc);



