CREATE TABLE [dbo].[TreeNodeDocuments] (
    [DocumentId] NVARCHAR (128) NOT NULL,
    [TreeNodeId] NVARCHAR (128) NOT NULL,
    CONSTRAINT [FK_dbo.TreeNodeDocuments] PRIMARY KEY CLUSTERED ([DocumentId] ASC, [TreeNodeId] ASC),
    CONSTRAINT [FK_dbo.TreeNodeDocuments_dbo.Documents_Id] FOREIGN KEY ([DocumentId]) REFERENCES [dbo].[Documents] ([Id]),
    CONSTRAINT [FK_dbo.TreeNodeDocuments_dbo.TreeNodes_Id] FOREIGN KEY ([TreeNodeId]) REFERENCES [dbo].[TreeNodes] ([Id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [TreeNodeIdIndex]
    ON [dbo].[TreeNodeDocuments]([TreeNodeId] ASC);

