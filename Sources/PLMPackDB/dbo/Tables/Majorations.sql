CREATE TABLE [dbo].[Majorations] (
    [MajorationSetId] NVARCHAR (128) NOT NULL,
    [Name]            NVARCHAR (128) NOT NULL,
    [Value]           FLOAT (53)     NOT NULL,
    CONSTRAINT [PK_dbo.Majorations] PRIMARY KEY CLUSTERED ([MajorationSetId] ASC, [Name] ASC),
    CONSTRAINT [FK_dbo.Majorations_dbo.MajorationSet_Id] FOREIGN KEY ([MajorationSetId]) REFERENCES [dbo].[MajorationSets] ([Id])
);

