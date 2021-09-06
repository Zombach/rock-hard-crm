CREATE PROCEDURE dbo.CommissionFee_SelectByRole
	@Role int
AS
BEGIN
	SELECT 
		c.Id,
		c.LeadId,
		c.AccountId,
		c.Date,
		c.Amount,
		c.Role
	FROM dbo.[CommissionFee] c
	Where c.Role=@Role
END