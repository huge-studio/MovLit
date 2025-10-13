CREATE TABLE [dbo].[BlogComment] (
    [BlogCommentId] INT            IDENTITY (1, 1) NOT NULL,
    [BlogId]        INT            NOT NULL,
    [Name]          NVARCHAR (50)  NOT NULL,
    [Email]         NVARCHAR (256) NOT NULL,
    [Comment]       NVARCHAR (MAX) NOT NULL,
    [IsPublished]   BIT            NOT NULL,
    [CreatedBy]     NVARCHAR (256) NOT NULL,
    [CreatedOn]     DATETIME2 (7)  NOT NULL,
    [ModifiedBy]    NVARCHAR (256) NOT NULL,
    [ModifiedOn]    DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_BlogComment] PRIMARY KEY CLUSTERED ([BlogCommentId] ASC),
    CONSTRAINT [FK_BlogConnent_Blog] FOREIGN KEY ([BlogId]) REFERENCES [dbo].[Blog] ([BlogId]) ON DELETE CASCADE
);

