CREATE PROCEDURE dbo.Lead_Delete
	@Id int
AS
BEGIN
    UPDATE dbo.[Lead]
    SET
        IsDeleted = 1
    WHERE Id = @Id
END