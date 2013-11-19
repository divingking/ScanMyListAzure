-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE DeleteInventoryByUpc
	@businessId int,
	@upc varchar(20)
AS
	delete from Inventory where businessId = @businessId and upc = @upc;
GO