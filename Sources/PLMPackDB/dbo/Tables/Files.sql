CREATE TABLE [dbo].[Files] (
    [Guid]        NVARCHAR (128) NOT NULL,
    [Extension]   NVARCHAR (5)   NULL,
    [DateCreated] DATE           NOT NULL,
    CONSTRAINT [PK_dbo.Files] PRIMARY KEY CLUSTERED ([Guid] ASC)
);

