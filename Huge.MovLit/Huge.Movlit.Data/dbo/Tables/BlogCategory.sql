CREATE TABLE [dbo].[BlogCategory] (
    [BlogCategoryId]       INT            IDENTITY (1, 1) NOT NULL,
    [BlogId]               INT            NOT NULL,
    [BlogCategorySourceId] INT            NOT NULL,
    [CreatedBy]            NVARCHAR (256) NOT NULL,
    [CreatedOn]            DATETIME2 (7)  NOT NULL,
    [ModifiedBy]           NVARCHAR (256) NOT NULL,
    [ModifiedOn]           DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_BlogCategory] PRIMARY KEY CLUSTERED ([BlogCategoryId] ASC),
    CONSTRAINT [FK_BlogCategory_Blog] FOREIGN KEY ([BlogId]) REFERENCES [dbo].[Blog] ([BlogId]) ON DELETE CASCADE,
    CONSTRAINT [FK_BlogCategory_BlogCategorySource] FOREIGN KEY ([BlogCategorySourceId]) REFERENCES [dbo].[BlogCategorySource] ([BlogCategorySourceId])
);

