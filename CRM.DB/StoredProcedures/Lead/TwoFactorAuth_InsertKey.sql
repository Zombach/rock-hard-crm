CREATE PROCEDURE [dbo].[TwoFactorAuth_InsertKey]
	@LeadID int,
	@TwoFactorKey nvarchar(10)
AS
BEGIN
	INSERT INTO dbo.TwoFactorAuth
	([LeadId],[TwoFactorKey])
	VALUES
	(@LeadID,@TwoFactorKey)
	SELECT @@IDENTITY
END