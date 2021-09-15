CREATE PROCEDURE [dbo].[Lead_SelectAllByBatchesWithoutAdmins]
@LastId int
AS
BEGIN
  SELECT 
  TOP 10000
    l.Id,
    l.FirstName,
    l.LastName,
    l.Patronymic,
    l.BirthDate,
    l.Email,
    a.Id,
    a.Currency,  
    l.Role as Id
  FROM dbo.[Lead] l
  left join dbo.Account a on a.LeadId = l.Id
  Where l.IsDeleted = 0
  AND l.Id > @LastId
  AND l.Role IN (2, 3)
END