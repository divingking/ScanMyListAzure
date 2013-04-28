-- ==========================================================
-- Create Stored Procedure Template for SQL Azure Database
-- ==========================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		K Jiang
-- Create date: 06/03/2012
-- Description:	Adding new product into the Product table
-- =============================================
CREATE PROCEDURE AddProduct
	-- Add the parameters for the stored procedure here
	@upc varchar(13) = 0, 
	@name varchar(30) = NULL, 
	@detail varchar(255) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	insert into Product values(@upc, @name, @detail, NULL);
END
