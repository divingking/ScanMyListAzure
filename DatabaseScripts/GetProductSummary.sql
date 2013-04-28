-- ==========================================================
-- Create Stored Procedure Template for SQL Azure Database
-- ==========================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		K Jiang
-- Create date: 07/15/2012
-- Description:	Get a summary of the orders of the product
-- =============================================
CREATE PROCEDURE GetProductSummary
	-- Add the parameters for the stored procedure here
	@cid int = -1, 
	@upc varchar(13) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	select c.oid, c.quantity, o.date
	from OrderList o, Contain c
	where c.upc = @upc and c.oid = o.id and o.cid = @cid
	order by o.date;
END
GO
