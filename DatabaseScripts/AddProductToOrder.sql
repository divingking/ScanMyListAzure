-- =============================================
-- Author:		K Jiang
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE AddProductToOrder
	-- Add the parameters for the stored procedure here
	@oid int = 0, 
	@upc varchar(13) = 0, 
	@supplier int = NULL, 
	@quantity int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	insert into Contain values(@oid, @upc, @supplier, @quantity);
END