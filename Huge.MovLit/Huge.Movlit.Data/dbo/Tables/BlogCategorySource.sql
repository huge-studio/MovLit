CREATE TABLE [dbo].[BlogCategorySource] (
    [BlogCategorySourceId] INT            IDENTITY (1, 1) NOT NULL,
    [ModuleId]             INT            NOT NULL,
    [Name]                 NVARCHAR (50)  NOT NULL,
    [CreatedBy]            NVARCHAR (256) NOT NULL,
    [CreatedOn]            DATETIME2 (7)  NOT NULL,
    [ModifiedBy]           NVARCHAR (256) NOT NULL,
    [ModifiedOn]           DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_BlogCategorySource] PRIMARY KEY CLUSTERED ([BlogCategorySourceId] ASC),
    CONSTRAINT [FK_BlogCategorySource_Module] FOREIGN KEY ([ModuleId]) REFERENCES [dbo].[Module] ([ModuleId]) ON DELETE CASCADE
);

