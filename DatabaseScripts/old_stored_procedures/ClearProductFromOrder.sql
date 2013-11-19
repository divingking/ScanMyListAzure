-- =============================================
-- Author:		K Jiang
-- Create date: 01/22/2013
-- Description:	Clear out order's product
-- =============================================

CREATE PROCEDURE ClearProductFromOrder
	-- Add the parameters for the stored procedure here
	@oid int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	delete from Contain
	where oid = @oid;
END