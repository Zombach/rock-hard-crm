CREATE PROCEDURE dbo.Lead_SelectAll
AS
BEGIN
	SELECT 
		l.Id,
		l.FirstName,
		l.LastName,
		l.Patronymic,
		l.BirthDate,
		l.RegistrationDate,
		l.Email,
		c.Id,
		c.Name,
		l.Role as Id
	FROM dbo.[Lead] l
	INNER JOIN City c on c.Id = l.CityId
	WHERE IsDeleted = 0
END