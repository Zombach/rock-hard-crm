CREATE TABLE [dbo].[Account] (
    [Id]        INT             NOT NULL IDENTITY (1, 1),
    [LeadId]    INT             NOT NULL,
    [Currency]  INT             NOT NULL,
    [CreatedOn] DATETIME2 (7)   NOT NULL,
    [Closed]    DATETIME2 (7),
    [IsDeleted] BIT             DEFAULT ('0'),
    CONSTRAINT [PK_ACCOUNT] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [Account_fk0] FOREIGN KEY ([LeadId]) REFERENCES [dbo].[Lead] ([Id]) ON UPDATE NO ACTION
);