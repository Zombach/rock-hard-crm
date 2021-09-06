CREATE PROCEDURE dbo.CommissionFee_SelectAll
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
END