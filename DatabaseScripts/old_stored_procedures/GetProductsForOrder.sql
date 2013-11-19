-- =============================================
-- Author:		K Jiang
-- Create date: 01/07/2013
-- Description:	Get the order details
-- =============================================

CREATE PROCEDURE GetProductsForOrder
	-- Add the parameters for the stored procedure here
	@order_id int = 0, 
	@customer_id int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select 
		p.upc, p.name as product_name, p.detail as product_detail, i.lead_time, i.location, 
		c.quantity, sup.price, 
		s.id as supplier_id, s.name as supplier_name, s.addr as supplier_addr, s.business
	from Contain c, Product p, Supplier s, Supplies sup, Inventory i
	where	c.oid = @order_id and c.upc = p.upc and 
			c.supplier = s.id and s.cid = @customer_id and c.supplier = sup.supplier and 
			sup.upc = p.upc and p.upc = i.upc and i.cid = @customer_id;
END