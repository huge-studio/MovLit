CREATE TABLE [dbo].[BlogTagSource] (
    [BlogTagSourceId] INT            IDENTITY (1, 1) NOT NULL,
    [ModuleId]        INT            NOT NULL,
    [Tag]             NVARCHAR (50)  NOT NULL,
    [CreatedBy]       NVARCHAR (256) NOT NULL,
    [CreatedOn]       DATETIME2 (7)  NOT NULL,
    [ModifiedBy]      NVARCHAR (256) NOT NULL,
    [ModifiedOn]      DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_BlogTagSource] PRIMARY KEY CLUSTERED ([BlogTagSourceId] ASC),
    CONSTRAINT [FK_BlogTagSource_Module] FOREIGN KEY ([ModuleId]) REFERENCES [dbo].[Module] ([ModuleId]) ON DELETE CASCADE
);

