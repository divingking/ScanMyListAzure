-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE CreateInventory
	-- Add the parameters for the stored procedure here
	@businessId int,
	@upc varchar(20),
	@name varchar(100),
	@defaultPrice decimal,
	@detail varchar(200),
	@leadTime int,
	@quantityAvailable int,
	@category int,
	@location varchar(40)
AS
	insert into Inventory values(@businessId, @upc, @name, @defaultPrice, @detail, @leadTime, @quantityAvailable, @category, @location);
GO