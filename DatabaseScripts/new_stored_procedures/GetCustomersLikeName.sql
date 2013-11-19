-- =============================================
-- Author:  Changhao Han
-- Create date: 11/18/2013
-- Description: 
-- =============================================
CREATE PROCEDURE GetCustomersLikeName
 @businessId int,
 @name varchar(100)
AS
 select Business.name, Business.postalCode, Customer.* from Business, Customer
 where Customer.businessId = @businessId and Business.id = Customer.customerId and Business.name like @name;
GO