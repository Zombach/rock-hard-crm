CREATE PROCEDURE dbo.CommissionFee_SeatchingByPeriod
	@From		datetime2,
	@To			datetime2,
	@LeadId		int = null,
	@AccountId  int = null,
	@Role		int = null
as
BEGIN
	SELECT 
		c.Id,
		c.LeadId,
		c.AccountId,
		c.TransactionId,
		c.Role,
		c.Date,
		c.Amount
	FROM dbo.CommissionFee c
		WHERE 
			c.Date BETWEEN @From and @To and 
			(c.AccountId = @AccountId OR @AccountId is null) and 
			(c.LeadId = @LeadId OR @LeadId is null) and 
			(c.Role = @Role OR @Role is null)
	ORDER BY c.Date
END