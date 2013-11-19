-- =============================================
-- Author:		K Jiang
-- Create date: 01/07/2013
-- Description:	Get the orders in given range
-- =============================================

CREATE PROCEDURE GetOrdersByRange
	-- Add the parameters for the stored procedure here
	@customer_id int = 0, 
	@start int = 0, 
	@end int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select * from (
		select Row_Number() over(order by date) as row_id, id, title, sent, scan_in, date
		from OrderList
		where cid = @customer_id
	) as orders
	where row_id >= @start and row_id <= @end;
END