CREATE PROCEDURE dbo.Lead_SelectById
	@Id int
AS
BEGIN
	SELECT 
		l.Id,
		l.FirstName,
		l.LastName,
		l.Patronymic,
		l.RegistrationDate,
		l.Email,
		l.PhoneNumber,
		l.IsDeleted,
		l.CityId	as Id,
		l.Role		as Id
	FROM dbo.[Lead] l
	WHERE l.Id = @Id 
END