CREATE TABLE [dbo].[City] (
    [Id]   INT           NOT NULL IDENTITY (1, 1),
    [Name] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_CITY] PRIMARY KEY CLUSTERED ([Id] ASC)
);

