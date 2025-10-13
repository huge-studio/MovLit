CREATE TABLE [dbo].[BlogTag] (
    [BlogTagId]       INT            IDENTITY (1, 1) NOT NULL,
    [BlogId]          INT            NOT NULL,
    [BlogTagSourceId] INT            NOT NULL,
    [CreatedBy]       NVARCHAR (256) NOT NULL,
    [CreatedOn]       DATETIME2 (7)  NOT NULL,
    [ModifiedBy]      NVARCHAR (256) NOT NULL,
    [ModifiedOn]      DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_BlogTag] PRIMARY KEY CLUSTERED ([BlogTagId] ASC),
    CONSTRAINT [FK_BlogTag_Blog] FOREIGN KEY ([BlogId]) REFERENCES [dbo].[Blog] ([BlogId]) ON DELETE CASCADE,
    CONSTRAINT [FK_BlogTag_BlogTagSource] FOREIGN KEY ([BlogTagSourceId]) REFERENCES [dbo].[BlogTagSource] ([BlogTagSourceId])
);

