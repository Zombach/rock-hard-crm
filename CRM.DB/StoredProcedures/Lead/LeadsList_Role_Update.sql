Create Procedure [dbo].[LeadsList_Role_Update]
  @tblLeadDto LeadDtoType readonly 
As
Begin
  Update dbo.Lead
  Set 
	LeadId = src.LeadId,
	Role = src.Role

  From 
	@tblLeadDto src
  Where 
	dbo.Lead.Id = src.LeadId
	and
	dbo.Lead.Role = src.Role
End