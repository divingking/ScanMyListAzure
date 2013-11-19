-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	count number of inventories
-- =============================================
CREATE PROCEDURE CountInventory
	-- Add the parameters for the stored procedure here
	@businessId int
AS
	select count(distinct upc) from Inventory where businessId = @businessId;
GO