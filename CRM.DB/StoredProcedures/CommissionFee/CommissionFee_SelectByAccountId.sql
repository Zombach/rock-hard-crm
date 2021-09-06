CREATE PROCEDURE dbo.CommissionFee_SelectByAccountId
	@AccountId int
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
	Where c.AccountId=@AccountId
END