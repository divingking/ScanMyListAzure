-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE CreateCustomer
	-- Add the parameters for the stored procedure here
	@businessId int,
	@customerId int,
	@address varchar(200),
	@email varchar(200),
	@phoneNumber varchar(20),
	@category int
AS
	insert into Customer values(@businessId, @customerId, @address, @email, @phoneNumber, @category);
GO