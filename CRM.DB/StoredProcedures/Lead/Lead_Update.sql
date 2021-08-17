CREATE PROCEDURE dbo.Lead_Update
	@Id				int,
	@FirstName		nvarchar(50),
	@LastName		nvarchar(50),
	@Patronymic		nvarchar(50),
	@Email			nvarchar(50),
	@PhoneNumber	nvarchar(12),
	@CityId			int
AS
BEGIN
	UPDATE dbo.[Lead]
    SET
		FirstName		= @FirstName,
		LastName		= @LastName,
		Patronymic		= @Patronymic,
		Email			= @Email,
		PhoneNumber		= @PhoneNumber,
		CityId			= @CityId
    WHERE Id = @Id
END