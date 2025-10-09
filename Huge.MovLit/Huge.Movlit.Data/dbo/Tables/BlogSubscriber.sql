CREATE TABLE [dbo].[BlogSubscriber] (
    [BlogSubscriberId] INT            IDENTITY (1, 1) NOT NULL,
    [ModuleId]         INT            NOT NULL,
    [Email]            NVARCHAR (256) NOT NULL,
    [Guid]             NVARCHAR (36)  NOT NULL,
    [CreatedBy]        NVARCHAR (256) NOT NULL,
    [CreatedOn]        DATETIME2 (7)  NOT NULL,
    [ModifiedBy]       NVARCHAR (256) NOT NULL,
    [ModifiedOn]       DATETIME2 (7)  NOT NULL,
    [IsVerified]       BIT            NULL,
    CONSTRAINT [PK_BlogSubscriber] PRIMARY KEY CLUSTERED ([BlogSubscriberId] ASC),
    CONSTRAINT [FK_BlogSubscribers_Module] FOREIGN KEY ([ModuleId]) REFERENCES [dbo].[Module] ([ModuleId]) ON DELETE CASCADE
);

