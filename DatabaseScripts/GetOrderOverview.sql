-- ==========================================================
-- Create Stored Procedure Template for SQL Azure Database
-- ==========================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		K Jiang
-- Create date: 01/22/2013
-- Description:	Get the overview of the order by order's id and customer's id
-- =============================================
CREATE PROCEDURE GetOrderOverview
	-- Add the parameters for the stored procedure here
	@cid int = -1, 
	@oid int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	select *
	from OrderList
	where cid = @cid and id = @oid;
END
GO
