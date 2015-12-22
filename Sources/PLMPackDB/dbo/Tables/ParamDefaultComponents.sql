CREATE TABLE [dbo].[ParamDefaultComponents] (
    [ComponentGuid] NVARCHAR (128)	NOT NULL,
    [GroupId]       NVARCHAR (128)	NOT NULL,
    [Name]          NVARCHAR (128)	NOT NULL,
    [Value]         FLOAT (53)		NOT NULL,
    CONSTRAINT [PK_dbo.ParamDefaultComponents] PRIMARY KEY CLUSTERED ([ComponentGuid] ASC, [GroupId] ASC, [Name] ASC),
    CONSTRAINT [FK_dbo.ParamDefaultComponents_dbo.Components_Guid] FOREIGN KEY ([ComponentGuid]) REFERENCES [dbo].[Components] ([Guid]),
    CONSTRAINT [FK_dbo.ParamDefaultComponents_dbo.Groups_Id] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Groups] ([Id])
);
