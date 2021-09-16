CREATE PROCEDURE dbo.Lead_SelectByEmail
	@Email nvarchar(50)
AS
BEGIN
	SELECT 
		l.Id,
		l.Email,
		l.Password,
		l.Role as Id
	FROM dbo.[Lead] l
	WHERE l.Email = @Email 
END