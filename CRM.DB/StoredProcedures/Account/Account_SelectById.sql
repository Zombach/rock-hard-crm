CREATE PROCEDURE dbo.Account_SelectById
	@Id int
AS
BEGIN
	SELECT 
		Id,
		LeadId,
		Currency,
		CreatedOn,
		Closed,
		IsDeleted
	FROM dbo.[Account]
	WHERE Id = @Id
END
