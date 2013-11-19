-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE UpdateCustomer
	-- Add the parameters for the stored procedure here
	@businessId int,
	@customerId int,
	@address varchar(200),
	@email varchar(200),
	@phoneNumber varchar(20),
	@category int
AS
	update Customer set
		address = @address,
		email = @email,
		phoneNumber = @phoneNumber,
		category = @category
	where businessId = @businessId and customerId = @customerId;
GO