-- =============================================
-- Author:  Changhao Han
-- Create date: 11/18/2013
-- Description: 
-- =============================================
CREATE PROCEDURE GetInventoriesLikeName
 @businessId int,
 @name varchar(100)
AS
 select * from Inventory
 where businessId = @businessId and name like @name;
GO