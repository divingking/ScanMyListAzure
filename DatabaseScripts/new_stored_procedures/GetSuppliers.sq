-- =============================================
-- Author:  Changhao Han
-- Create date: 11/18/2013
-- Description: 
-- =============================================
CREATE PROCEDURE GetSuppliers
 @businessId int
AS
 select Business.name, Business.postalCode, Supplier.* from Business, Supplier where Supplier.businessId = @businessId and Business.id = Supplier.supplierId;
GO