-- =============================================
-- Author:		K Jiang
-- Create date: 
-- Description:	
-- =============================================

CREATE PROCEDURE GetOrders 
	-- Add the parameters for the stored procedure here
	@customer_id int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT o.cid as cid, o.id as oid, o.title, o.date, o.sent, o.scan_in, p.upc, p.name as pname, p.detail, i.lead_time, i.location, h.quantity, s.price, sup.id as sid, sup.name as sname, sup.addr as saddr
	FROM OrderList o, Contain h, Product p, Inventory i, Supplies s, Supplier sup
	WHERE	o.cid = @customer_id and 
			h.oid = o.id and h.upc = p.upc and 
			p.upc = i.upc and i.cid = @customer_id and 
			s.supplier = h.supplier and s.upc = h.upc and 
			sup.id = h.supplier and sup.cid = @customer_id
	ORDER BY o.id;
END