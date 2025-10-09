CREATE TABLE [dbo].[BlogContent] (
    [BlogContentId] INT             IDENTITY (1, 1) NOT NULL,
    [BlogId]        INT             NOT NULL,
    [Version]       INT             NOT NULL,
    [Summary]       NVARCHAR (2000) NOT NULL,
    [Content]       NVARCHAR (MAX)  NOT NULL,
    [IsPublished]   BIT             NOT NULL,
    [PublishDate]   DATETIME2 (7)   NULL,
    [CreatedBy]     NVARCHAR (256)  NOT NULL,
    [CreatedOn]     DATETIME2 (7)   NOT NULL,
    [ModifiedBy]    NVARCHAR (256)  NOT NULL,
    [ModifiedOn]    DATETIME2 (7)   NOT NULL,
    CONSTRAINT [PK_BlogContent] PRIMARY KEY CLUSTERED ([BlogContentId] ASC),
    CONSTRAINT [FK_BlogContent_Blog] FOREIGN KEY ([BlogId]) REFERENCES [dbo].[Blog] ([BlogId]) ON DELETE CASCADE
);

