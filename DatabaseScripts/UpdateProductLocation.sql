-- =============================================
-- Author:		K Jiang
-- Create date: 01/31/2013
-- Description:	Update the location of a product
-- =============================================
CREATE PROCEDURE UpdateProductLocation
	-- Add the parameters for the stored procedure here
	@upc varchar(13) = NULL, 
	@customer_id int = 0, 
	@location varchar(20) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	update Inventory
	set location = @location
	where upc = @upc and cid = @customer_id;
END
GO
