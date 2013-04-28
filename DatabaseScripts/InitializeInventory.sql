-- =============================================
-- Author:		K Jiang
-- Create date: 01/19/2013
-- Description:	Initializes the Inventory for Product
-- =============================================
CREATE PROCEDURE InitializeInventory
	-- Add the parameters for the stored procedure here
	@upc varchar(13) = NULL, 
	@customer_id int = 0, 
	@lead_time int = 0, 
	@quantity int = 0, 
	@location varchar(20) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	insert into Inventory values(@upc, @customer_id, @lead_time, @quantity, @location);
END
GO
