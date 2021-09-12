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
    a.Id,
    a.Currency,
    a.CreatedOn,
    c.Id,
    c.Name,  
    l.Role as Id
  FROM dbo.[Lead] l
  left join dbo.Account a on a.LeadId = l.Id
  inner join dbo.City c on c.Id = l.CityId
  Where l.IsDeleted = 0
END