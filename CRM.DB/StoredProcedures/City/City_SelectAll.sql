CREATE PROCEDURE dbo.City_SelectAll
AS
BEGIN
	SELECT 
		Id,
		Name
	FROM dbo.City
END
