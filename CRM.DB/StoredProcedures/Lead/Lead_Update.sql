CREATE PROCEDURE dbo.Lead_Update
	@Id				INT,
	@FirstName		nvarchar(50),
	@LastName		nvarchar(50),
	@Patronymic		nvarchar(50),
	@Email			nvarchar(50),
	@PhoneNumber	nvarchar(12),
	@BirthDate		DATE
AS
BEGIN
	UPDATE dbo.[Lead]
    SET
		FirstName		= @FirstName,
		LastName		= @LastName,
		Patronymic		= @Patronymic,
		Email			= @Email,
		PhoneNumber		= @PhoneNumber,
		BirthDate		= @BirthDate
    WHERE Id = @Id
END