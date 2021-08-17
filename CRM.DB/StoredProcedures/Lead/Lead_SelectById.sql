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
		a.Id,
		a.Currency,
		a.CreatedOn,
		l.CityId	as Id,
		l.Role		as Id
	FROM dbo.[Lead] l
	left join dbo.Account a on a.LeadId = l.Id
	WHERE l.Id = @Id 
END