CREATE TABLE [dbo].[TwoFactorAuth]
(
	[Id] INT NOT NULL IDENTITY (1, 1), 
    [LeadId] INT NULL, 
    [TwoFactorKey] NCHAR(10) NULL
    CONSTRAINT [PK_TwoFactorAuth] PRIMARY KEY CLUSTERED ([Id] ASC)

)
