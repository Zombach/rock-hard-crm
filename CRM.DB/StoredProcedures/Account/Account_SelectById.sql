CREATE PROCEDURE dbo.Account_SelectById
	@Id int
AS
BEGIN
	SELECT 
		Id,
		LeadId,
		CreatedOn,
		Closed,
		IsDeleted,
		Currency AS Id
	FROM dbo.[Account]
	WHERE Id = @Id
END
