-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

CREATE PROCEDURE GetProductByName
	@name varchar(30) = NULL, 
	@customer_id int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT p.upc, p.name, p.detail, i.lead_time, i.quantity, i.location
	FROM Product p, Inventory i
	WHERE p.name like @name and i.upc = p.upc and i.cid = @customer_id;
END