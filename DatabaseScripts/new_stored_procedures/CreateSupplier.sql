-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE CreateSupplier
	-- Add the parameters for the stored procedure here
	@businessId int,
	@supplierId int,
	@address varchar(200),
	@email varchar(200),
	@phoneNumber varchar(20),
	@category int
AS
	insert into Supplier values(@businessId, @supplierId, @address, @email, @phoneNumber, @category);
GO