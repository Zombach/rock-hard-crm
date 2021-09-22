CREATE PROCEDURE [dbo].[Lead_SelectKeyByLeadId]
	@LeadId int	
AS
BEGIN
	SELECT TwoFactorKey
	FROM dbo.[TwoFactorAuth] 
 	WHERE LeadId=@LeadId
END
