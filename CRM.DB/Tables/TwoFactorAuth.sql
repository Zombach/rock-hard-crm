CREATE TABLE [dbo].[TwoFactorAuth]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [LeadId] INT NULL, 
    [TwoFactorKey] NCHAR(10) NULL
)
