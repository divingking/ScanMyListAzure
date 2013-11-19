-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE GetAccounts
	@businessId int
AS
	select * from Account where businessId = @businessId;
GO