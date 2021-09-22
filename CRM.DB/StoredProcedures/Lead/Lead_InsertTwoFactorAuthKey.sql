CREATE PROCEDURE [dbo].[Lead_InsertTwoFactorAuthKey]
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