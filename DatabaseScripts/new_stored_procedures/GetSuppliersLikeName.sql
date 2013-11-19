-- =============================================
-- Author:  Changhao Han
-- Create date: 11/18/2013
-- Description: 
-- =============================================
CREATE PROCEDURE GetSuppliersLikeName
 @businessId int,
 @name varchar(100)
AS
 select Business.name, Business.postalCode, Supplier.* from Business, Supplier
 where Supplier.businessId = @businessId and Business.id = Supplier.supplierId and Business.name like @name;
GO