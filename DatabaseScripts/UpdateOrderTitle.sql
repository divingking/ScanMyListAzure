-- =============================================
-- Author:		K Jiang
-- Create date: 01/19/2013
-- Description:	Update the order's title
-- =============================================

CREATE PROCEDURE UpdateOrderTitle
	-- Add the parameters for the stored procedure here
	@customer_id int = 0, 
	@oid int = 0, 
	@title varchar(100) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	update OrderList
	set title = @title
	where cid = @customer_id and id = @oid;
END