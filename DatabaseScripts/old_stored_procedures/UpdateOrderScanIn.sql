-- =============================================
-- Author:		K Jiang
-- Create date: 01/22/2013
-- Description:	Update the order's scan_in
-- =============================================

CREATE PROCEDURE UpdateOrderScanIn
	-- Add the parameters for the stored procedure here
	@customer_id int = 0, 
	@oid int = 0, 
	@scan_in bit = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	update OrderList
	set scan_in = @scan_in
	where cid = @customer_id and id = @oid;
END