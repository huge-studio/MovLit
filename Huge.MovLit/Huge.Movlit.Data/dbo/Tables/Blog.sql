CREATE TABLE [dbo].[Blog] (
    [BlogId]        INT            IDENTITY (1, 1) NOT NULL,
    [ModuleId]      INT            NOT NULL,
    [Title]         NVARCHAR (256) NOT NULL,
    [CreatedBy]     NVARCHAR (256) NOT NULL,
    [CreatedOn]     DATETIME2 (7)  NOT NULL,
    [ModifiedBy]    NVARCHAR (256) NOT NULL,
    [ModifiedOn]    DATETIME2 (7)  NOT NULL,
    [Slug]          NVARCHAR (255) NULL,
    [Thumbnail]     INT            DEFAULT ((-1)) NOT NULL,
    [AlternateText] NVARCHAR (200) NULL,
    [Views]         INT            DEFAULT ((0)) NOT NULL,
    [AllowComments] BIT            NULL,
    CONSTRAINT [PK_Blog] PRIMARY KEY CLUSTERED ([BlogId] ASC),
    CONSTRAINT [FK_Blog_Module] FOREIGN KEY ([ModuleId]) REFERENCES [dbo].[Module] ([ModuleId]) ON DELETE CASCADE
);

