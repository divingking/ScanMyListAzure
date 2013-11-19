-- =============================================
-- Author:  Changhao Han
-- Create date: 11/18/2013
-- Description: 
-- =============================================
CREATE PROCEDURE GetInventoryByUpc
 @businessId int,
 @upc varchar(20)
AS
 select * from Inventory
 where businessId = @businessId and upc = @upc;
GO