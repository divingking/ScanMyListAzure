-- =============================================
-- Author:  Changhao Han
-- Create date: 11/18/2013
-- Description: 
-- =============================================
CREATE PROCEDURE PageCustomers
 @businessId int,
 @pageSize int,
 @offset int
AS
 select Business.name, Business.postalCode, Customer.* from Business, Customer
 where Customer.businessId = @businessId and Business.id = Customer.customerId order by Business.name offset @offset rows fetch next @pageSize rows only;
GO