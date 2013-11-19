-- =============================================
-- Author:		K Jiang
-- Create date: 06/18/2012
-- Description:	Get the count of the product in orders
-- =============================================
CREATE PROCEDURE ProductCount
	-- Add the parameters for the stored procedure here
	@upc varchar(13) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select upc, sum(quantity) as count
	from Contain
	where upc = @upc
	group by upc;
END
