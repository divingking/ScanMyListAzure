-- ==========================================================
-- Create Stored Procedure Template for SQL Azure Database
-- ==========================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		K Jiang
-- Create date: 01/11/2013
-- Description:	Get the full details of an order by order's id and customer's id
-- =============================================
CREATE PROCEDURE GetOrder
	-- Add the parameters for the stored procedure here
	@cid int = -1, 
	@oid int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	select o.title, o.date, o.sent, o.scan_in, p.upc, p.name as pname, p.detail, i.lead_time, c.quantity, sup.price, s.id as sid, s.name as sname, s.business, s.addr as saddr
	from OrderList o, Product p, Inventory i, Contain c, Supplier s, Supplies sup
	where 
		o.id = @oid and o.cid = @cid and 
		c.oid = o.id and c.supplier = s.id and s.cid = @cid and 
		c.upc = p.upc and p.upc = i.upc and i.cid = @cid and 
		sup.supplier = s.id and sup.upc = p.upc;
END
GO
