-- =============================================
-- Author:		K Jiang
-- Create date: 01/11/2013
-- Description:	Get the specified customer
-- =============================================

CREATE PROCEDURE GetCustomer 
	-- Add the parameters for the stored procedure here
	@customer_id int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select fname, mname, lname, email
	from Customer
	where id = @customer_id;
END