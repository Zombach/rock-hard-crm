CREATE PROCEDURE dbo.CommissionFee_SelectByLeadId
	@LeadId int
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
	Where c.LeadId=@LeadId
END