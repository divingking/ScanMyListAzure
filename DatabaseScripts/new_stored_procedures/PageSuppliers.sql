-- =============================================
-- Author:  Changhao Han
-- Create date: 11/18/2013
-- Description: 
-- =============================================
CREATE PROCEDURE PageSuppliers
 @businessId int,
 @pageSize int,
 @offset int
AS
 select Business.name, Business.postalCode, Supplier.* from Business, Supplier
 where Supplier.businessId = @businessId and Business.id = Supplier.supplierId order by Business.name offset @offset rows fetch next @pageSize rows only;
GO