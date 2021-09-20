Create Procedure [dbo].[Account_SelectByListId]
  @tblIds [dbo].[AccountIdType] readonly
AS
BEGIN
  SELECT 
	Id,
	LeadId,
	CreatedOn,
	Closed,
	IsDeleted,
	Currency AS Id
  FROM 
	dbo.Account a
	inner join @tblIds ids
	on a.Id=ids.AccountId
End