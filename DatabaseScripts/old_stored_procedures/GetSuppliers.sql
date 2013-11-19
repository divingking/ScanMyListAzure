-- =============================================
-- Author:		K Jiang
-- Create date: 
-- Description:	
-- =============================================

CREATE PROCEDURE GetSuppliers 
	-- Add the parameters for the stored procedure here
	@upc varchar(13) = NULL, 
	@cid int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT s.*, so.price
	FROM Supplier s, Supplies so
	WHERE so.upc = @upc AND s.id = so.supplier and s.cid = @cid;
END