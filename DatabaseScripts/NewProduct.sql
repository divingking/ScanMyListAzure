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
-- Description:	Create new product in our own database
-- =============================================
CREATE PROCEDURE NewProduct
	-- Add the parameters for the stored procedure here
	@upc varchar(13) = null, 
	@name varchar(30) = null, 
	@detail varchar(255) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	insert into Product values(@upc, @name, @detail, null);
END
