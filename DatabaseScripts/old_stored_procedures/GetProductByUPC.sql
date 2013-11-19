-- =============================================
-- Author:		K Jiang
-- Create date: 01/19/2013
-- Description:	Get Product details based on UPC
-- =============================================

CREATE PROCEDURE GetProductByUPC
	@upc varchar(13) = NULL, 
	@customer_id int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT p.name, p.detail, i.lead_time, i.quantity, i.location
	FROM Product p, Inventory i
	WHERE p.upc = @upc and i.upc = @upc and i.cid = @customer_id;
END