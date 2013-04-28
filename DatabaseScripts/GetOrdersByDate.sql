-- =============================================
-- Author:		K Jiang
-- Create date: 01/22/2013
-- Description:	Get orders made by customer within time range
-- =============================================
CREATE PROCEDURE GetOrdersByDate
	-- Add the parameters for the stored procedure here
	@customer_id int = 0, 
	@start bigint = 0, 
	@end bigint = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select *
	from OrderList
	where date >= @start and date <= @end;
END
GO
