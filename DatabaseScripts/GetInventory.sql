-- =============================================
-- Author:		K Jiang
-- Create date: 01/08/2013
-- Description:	Get n orders from last
-- =============================================

CREATE PROCEDURE GetInventory
	-- Add the parameters for the stored procedure here
	@customer_id int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	select p.upc, p.name, p.detail, i.lead_time, i.quantity, i.location
	from Product p, Inventory i
	where i.cid = @customer_id and i.upc = p.upc;
END