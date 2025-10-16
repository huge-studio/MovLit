CREATE TABLE [dbo].[Story] (
    [StoryId]     INT             IDENTITY (1, 1) NOT NULL,
    [ModuleId]    INT             NULL,
    [AuthorId]    INT             NULL,
    [AuthorName]  NVARCHAR (200)  NULL,
    [Title]       NVARCHAR (300)  NULL,
    [CoverArtUrl] NVARCHAR (2083) NULL,
    [Description] NVARCHAR (MAX)  NULL,
    [InkJson]     NVARCHAR (MAX)  NULL,
    [UpvoteCount] INT             NULL,
    [CreatedBy]   NVARCHAR (100)  NULL,
    [CreatedOn]   DATETIME2 (0)   NULL,
    [ModifiedBy]  NVARCHAR (100)  NULL,
    [ModifiedOn]  DATETIME2 (0)   NULL,
    [PageId]      INT             NOT NULL,
    [ViewCount]   INT             CONSTRAINT [DF_Story_ViewCount] DEFAULT ((0)) NOT NULL,
    [Tags]        NVARCHAR (MAX)  NULL
);




GO
CREATE NONCLUSTERED INDEX [IX_Story_ViewCount]
    ON [dbo].[Story]([ViewCount] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Story_CreatedOn]
    ON [dbo].[Story]([CreatedOn] ASC);

