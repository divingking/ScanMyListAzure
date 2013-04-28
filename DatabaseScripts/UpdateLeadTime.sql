-- =============================================
-- Author:		K Jiang
-- Create date: 01/19/2013
-- Description:	Update the lead time of a product
-- =============================================
CREATE PROCEDURE UpdateLeadTime
	-- Add the parameters for the stored procedure here
	@upc varchar(13) = NULL, 
	@customer_id int = 0, 
	@lead_time int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	update Inventory
	set lead_time = @lead_time
	where upc = @upc and cid = @customer_id;
END
GO
