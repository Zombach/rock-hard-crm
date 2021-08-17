CREATE PROCEDURE dbo.Lead_SelectByEmail
	@Email nvarchar(50)
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
		l.Password,
		l.IsDeleted,
		a.Id,
		a.Currency,
		a.CreatedOn,
		c.Id,
		c.Name,	
		l.Role as Id
	FROM dbo.[Lead] l
	left join dbo.Account a on a.LeadId = l.Id
	inner join dbo.City c on c.Id = l.CityId
	WHERE l.Email = @Email 
END