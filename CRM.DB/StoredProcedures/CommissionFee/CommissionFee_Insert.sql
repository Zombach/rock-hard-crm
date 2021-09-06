CREATE PROCEDURE dbo.CommissionFee_Insert
	@LeadId			INT,
	@AccountId		INT,
	@TransactionId	INT,
	@Amount 		DECIMAL (14,3),
	@Role			INT,
	@Date			datetime2
AS
BEGIN
	INSERT INTO dbo.[CommissionFee] 
		([LeadId],[AccountId],[TransactionId],[Amount],[Role],[Date])
	VALUES 
		(@LeadId, @AccountId, @TransactionId, @Amount, @Role, getdate())
	SELECT @@IDENTITY
END