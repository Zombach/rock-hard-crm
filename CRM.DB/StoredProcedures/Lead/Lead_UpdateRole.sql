CREATE PROCEDURE dbo.Lead_UpdateRole
	@Id		INT,
	@Role	INT
AS
BEGIN
	UPDATE dbo.[Lead]
    SET
		Role = @Role
    WHERE Id = @Id
END