-- =============================================
-- Author:		K Jiang
-- Create date: 01/19/2013
-- Description:	Check if customer has product in inventory
-- =============================================
CREATE PROCEDURE HasInventory
	-- Add the parameters for the stored procedure here
	@upc varchar(13) = NULL, 
	@customer_id int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	declare @lt int
	
	select @lt = count(*) 
	from Inventory 
	where upc = @upc and cid = @customer_id

	return @lt
END
GO
