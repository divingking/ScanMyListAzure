-- ==========================================================
-- Create Stored Procedure Template for SQL Azure Database
-- ==========================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		K Jiang
-- Create date: 01/11/2013
-- Description:	Mark order as sent
-- =============================================
CREATE PROCEDURE SendOrder
	-- Add the parameters for the stored procedure here
	@cid int = -1, 
	@oid int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	update OrderList
	set sent = 1, date = CONVERT(BIGINT, LEFT(REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR(30),CURRENT_TIMESTAMP,126),'-',''),':',''),'T',''),14))
	where cid = @cid and id = @oid;
END
GO
