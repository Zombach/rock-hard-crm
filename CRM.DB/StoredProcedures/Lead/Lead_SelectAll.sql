CREATE PROCEDURE dbo.Lead_SelectAll
AS
BEGIN
	SELECT 
		l.Id,
		l.FirstName,
		l.LastName,
		l.Patronymic,
		l.Email,
		l.BirthDate,
		c.id,
		c.Name,
		l.Role as Id
	FROM dbo.[Lead] l
	INNER JOIN City c on c.id = l.CityId
	WHERE IsDeleted = 0
END