CREATE PROCEDURE dbo.CommissionFee_SelectByLeadId
	@LeadId int
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
	Where c.LeadId=@LeadId
END