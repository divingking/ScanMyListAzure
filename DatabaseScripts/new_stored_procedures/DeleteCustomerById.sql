-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE DeleteCustomerById
	@customerId int
AS
	delete from Customer where customerId = @customerId;
GO