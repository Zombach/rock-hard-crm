CREATE PROCEDURE dbo.CommissionFee_SelectByPeriod
	@From datetime2,
	@To	  datetime2
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
	WHERE c.[Date] BETWEEN @From and @To
END