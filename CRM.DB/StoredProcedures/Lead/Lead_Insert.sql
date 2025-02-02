﻿CREATE PROCEDURE dbo.Lead_Insert
	@FirstName			nvarchar(50),
	@LastName			nvarchar(50),
	@Patronymic 		nvarchar(50),
	@Email				nvarchar(50),
	@PhoneNumber		nvarchar(12),
	@Password			nvarchar(200),                            
	@Role				INT,
	@CityId				INT,
	@BirthDate			DATE,
	@BirthYear			smallint,
	@BirthMonth			tinyint,
	@BirthDay			tinyint
AS
BEGIN
	INSERT INTO dbo.[Lead] 
		([FirstName],
		[LastName],
		[Patronymic],
		[RegistrationDate],
		[Email],
		[PhoneNumber],
		[Password],
		[Role],		
		[CityId],
		[BirthDate],
		[BirthYear],
		[BirthMonth],
		[BirthDay])
	VALUES 
		(@FirstName,
		@LastName,
		@Patronymic,
		getdate(),
		@Email,
		@PhoneNumber,
		@Password,
		@Role,
		@CityId,
		@BirthDate,
		@BirthYear,
		@BirthMonth,
		@BirthDay)
	SELECT @@IDENTITY
END