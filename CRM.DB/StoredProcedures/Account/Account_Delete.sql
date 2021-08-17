CREATE PROCEDURE dbo.Account_Delete
	@Id int
AS
BEGIN
    UPDATE dbo.[Account]
    SET
        IsDeleted = 1,
        Closed = getdate()
    WHERE Id = @Id
END