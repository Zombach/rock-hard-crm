CREATE PROCEDURE dbo.Account_Insert
	@LeadId			int,
	@Currency		int
AS
BEGIN
	INSERT INTO dbo.[Account] 
		([LeadId],[Currency],[CreatedOn])
	VALUES 
		(@LeadId, @Currency, getdate())
	SELECT @@IDENTITY
END