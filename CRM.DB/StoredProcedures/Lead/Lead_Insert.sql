CREATE PROCEDURE dbo.Lead_Insert
	@FirstName			nvarchar(50),
	@LastName			nvarchar(50),
	@Patronymic 		nvarchar(50),
	@Email				nvarchar(50),
	@PhoneNumber		nvarchar(12),
	@Password			nvarchar(200),                            
	@Role				INT,
	@CityId				INT,
	@BirthDate			DATE
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
		[BirthDate],
		[CityId])
	VALUES 
		(@FirstName,
		@LastName,
		@Patronymic,
		getdate(),
		@Email,
		@PhoneNumber,
		@Password,
		@Role,
		@BirthDate,
		@CityId)
	SELECT @@IDENTITY
END