-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE GetInventories
	@businessId int
AS
	select * from Inventory where businessId = @businessId;
GO