-- =============================================
-- Author:  Changhao Han
-- Create date: 11/18/2013
-- Description: 
-- =============================================
CREATE PROCEDURE PageInventories
 @businessId int,
 @pageSize int,
 @offset int
AS
 select * from Inventory
 where businessId = @businessId order by name offset @offset rows fetch next @pageSize rows only;
GO