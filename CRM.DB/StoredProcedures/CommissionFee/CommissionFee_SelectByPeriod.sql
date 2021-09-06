CREATE PROCEDURE dbo.CommissionFee_SelectByPeriod
	@From datetime2,
	@To	  datetime2
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
	WHERE c.[Date] BETWEEN @From and @To
END