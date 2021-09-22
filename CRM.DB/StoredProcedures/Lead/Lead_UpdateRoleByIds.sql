CREATE PROCEDURE [dbo].[LeadsList_Role_Update]
  @tblLeadDto LeadDtoType readonly 
AS
BEGIN
  UPDATE dbo.[Lead]
  SET 
	Role = src.Role

  FROM 
	@tblLeadDto src
  WHERE 
	dbo.Lead.Id = src.LeadId
End
