CREATE TABLE [dbo].[Components] (
    [Guid]       NVARCHAR (128) NOT NULL,
    [DocumentId] NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_dbo.Components] PRIMARY KEY CLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.Components_dbo.Documents_Id] FOREIGN KEY ([DocumentId]) REFERENCES [dbo].[Documents] ([Id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [ComponentGuidIndex]
    ON [dbo].[Components]([Guid] ASC);

