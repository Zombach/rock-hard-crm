CREATE PROCEDURE [dbo].[TwoFactorAuth_SelectKeyByLeadId]
	@LeadId int	
AS
BEGIN
	SELECT TwoFactorKey
	FROM dbo.[TwoFactorAuth] 
 	WHERE LeadId=@LeadId
END
