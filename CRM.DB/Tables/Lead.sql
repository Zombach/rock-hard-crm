CREATE TABLE [dbo].[Lead] (
    [Id]               INT            NOT NULL IDENTITY (1, 1),
    [FirstName]        NVARCHAR (50)  NOT NULL,
    [LastName]         NVARCHAR (50)  NOT NULL,
    [Patronymic]       NVARCHAR (50)  NOT NULL,
    [RegistrationDate] DATETIME2 (7)  NOT NULL,
    [Email]            NVARCHAR (50)  NOT NULL UNIQUE,
    [PhoneNumber]      NVARCHAR (12)  NOT NULL,
    [Password]         NVARCHAR (200) NOT NULL,
    [Role]             INT            NOT NULL,
    [CityId]           INT            NOT NULL,
    [IsDeleted]        BIT            DEFAULT ('0') NOT NULL,
    [BirthDate]        DATE           NOT NULL
    CONSTRAINT [PK_LEAD] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [Lead_fk0] FOREIGN KEY ([CityId]) REFERENCES [dbo].[City] ([Id]) ON UPDATE NO ACTION
);