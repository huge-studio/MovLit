CREATE TABLE [dbo].[StoryView] (
    [ViewId]     INT            IDENTITY (1, 1) NOT NULL,
    [StoryId]    INT            NULL,
    [UserId]     INT            NULL,
    [VisitorId]  INT            NULL,
    [CreatedBy]  NVARCHAR (100) NULL,
    [CreatedOn]  DATETIME2 (0)  NULL,
    [ModifiedBy] NVARCHAR (100) NULL,
    [ModifiedOn] DATETIME2 (0)  NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_StoryView_Story_Created]
    ON [dbo].[StoryView]([StoryId] ASC, [CreatedOn] ASC);

