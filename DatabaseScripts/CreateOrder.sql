-- =============================================
-- Author:		K Jiang
-- Create date: 
-- Description:	Creates a new order
-- =============================================
CREATE PROCEDURE CreateOrder
	-- Add the parameters for the stored procedure here
	@title varchar(100) = null,
	@cid int = 0, 
	@sent bit = 0, 
	@in bit = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	insert into OrderList values(@title, @cid, @sent, @in, CONVERT(BIGINT, LEFT(REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR(30),CURRENT_TIMESTAMP,126),'-',''),':',''),'T',''),14)));

	RETURN (SELECT IDENT_CURRENT('OrderList'))
END
