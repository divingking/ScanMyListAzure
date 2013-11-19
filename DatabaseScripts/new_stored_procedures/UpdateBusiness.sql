-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE UpdateBusiness
	-- Add the parameters for the stored procedure here
	@businessId int,
	@name varchar(100),
	@integration int,
	@tier int,
	@address varchar(200),
	@postalCode varchar(10),
	@email varchar(200),
	@phoneNumber varchar(20)
AS
	update Business set
		name = @name,
		integration = @integration,
		tier = @tier,
		address = @address,
		postalCode = @postalCode,
		email = @email,
		phoneNumber = @phoneNumber
	where id = @businessId;
GO