CREATE PROCEDURE dbo.Lead_SelectAll
AS
BEGIN
	SELECT 
		l.Id,
		l.FirstName,
		l.LastName,
		l.Patronymic,
		l.Email,
		l.Role as Id
	FROM dbo.[Lead] l
	WHERE IsDeleted = 0
END