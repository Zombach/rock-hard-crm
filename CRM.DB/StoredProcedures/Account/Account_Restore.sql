CREATE PROCEDURE dbo.Account_Restore
	@Id int
AS
BEGIN
    UPDATE dbo.[Account]
    SET
        IsDeleted = 0,
        Closed = NULL
    WHERE Id = @Id
END