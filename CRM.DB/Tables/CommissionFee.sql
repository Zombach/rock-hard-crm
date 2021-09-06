CREATE TABLE [dbo].[CommissionFee]
(
	[Id]               INT            NOT NULL IDENTITY (1, 1),
    [LeadId]           INT            NOT NULL,
    [AccountId]        INT            NOT NULL,
    [TransactionId]    INT            NOT NULL,
    [Role]             INT            NOT NULL,
    [Date]             DATETIME2 (7)  NOT NULL,
    [Amount]           DECIMAL (14,3) NOT NULL,
    CONSTRAINT [PK_СommissionFee] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [СommissionFee_fk0] FOREIGN KEY ([LeadId]) REFERENCES [dbo].[Lead] ([Id]) ON UPDATE NO ACTION,
    CONSTRAINT [СommissionFee_fk1] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Account] ([Id]) ON UPDATE NO ACTION
)