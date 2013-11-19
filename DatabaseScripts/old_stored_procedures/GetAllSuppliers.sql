-- ==========================================================
-- Create Stored Procedure Template for SQL Azure Database
-- ==========================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		K Jiang
-- Create date: 06/05/2012
-- Description:	Get all suppliers in the table
-- =============================================
CREATE PROCEDURE GetAllSuppliers
	@cid int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select * from Supplier where cid = @cid;
END
