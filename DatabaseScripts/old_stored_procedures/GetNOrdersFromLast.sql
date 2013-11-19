-- =============================================
-- Author:		K Jiang
-- Create date: 01/08/2013
-- Description:	Get n orders from last
-- =============================================

CREATE PROCEDURE GetNOrdersFromLast
	-- Add the parameters for the stored procedure here
	@customer_id int = 0, 
	@last int = 0, 
	@n int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	select top (@n) o.id, o.title, o.cid, o.sent, o.scan_in, o.date
	from 
		(select * from OrderList where id = @last and cid = @customer_id) l, 
		OrderList o
	where o.date > l.date and o.cid = @customer_id;
END