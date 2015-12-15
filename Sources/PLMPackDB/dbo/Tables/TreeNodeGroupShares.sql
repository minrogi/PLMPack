CREATE TABLE [dbo].[TreeNodeGroupShares] (
    [TreeNodeId] NVARCHAR (128) NOT NULL,
    [GroupId]    NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_dbo.TreeNodeGroupShares] PRIMARY KEY NONCLUSTERED ([GroupId] ASC, [TreeNodeId] ASC),
    CONSTRAINT [FK_dbo.TreeNodeGroupShares_dbo.Groups_Id] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Groups] ([Id]),
    CONSTRAINT [FK_dbo.TreeNodeGroupShares_dbo.TreeNodes_Id] FOREIGN KEY ([TreeNodeId]) REFERENCES [dbo].[TreeNodes] ([Id])
);
GO
CREATE CLUSTERED INDEX IX_TREENODEGROUPSHARES ON TreeNodeGroupShares
(TreeNodeId Asc, GroupId Asc);
