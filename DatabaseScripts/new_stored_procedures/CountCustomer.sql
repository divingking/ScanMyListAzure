-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	count number of customers
-- =============================================
CREATE PROCEDURE CountCustomer
	-- Add the parameters for the stored procedure here
	@businessId int
AS
	select count(distinct customerId) from Customer where businessId = @businessId;
GO