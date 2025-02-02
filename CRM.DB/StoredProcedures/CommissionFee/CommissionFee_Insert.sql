﻿CREATE PROCEDURE dbo.CommissionFee_Insert
	@LeadId			INT,
	@AccountId		INT,
	@TransactionId	BIGINT,
	@Amount 		DECIMAL (14,3),
	@Role			INT
AS
BEGIN
	INSERT INTO dbo.[CommissionFee] 
		([LeadId],[AccountId],[TransactionId],[Amount],[Role],[Date])
	VALUES 
		(@LeadId, @AccountId, @TransactionId, @Amount, @Role, getdate())
	SELECT @@IDENTITY
END