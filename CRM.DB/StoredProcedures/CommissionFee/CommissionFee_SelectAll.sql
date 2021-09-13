CREATE PROCEDURE dbo.CommissionFee_SelectAll
AS
BEGIN
	SELECT 
		c.Id,
		c.LeadId,
		c.AccountId,
		c.TransactionId,
		c.Role,
		c.Date,
		c.Amount
	FROM dbo.[CommissionFee] c
END