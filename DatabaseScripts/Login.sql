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
-- Description:	Login process
-- =============================================
CREATE PROCEDURE Login
	-- Add the parameters for the stored procedure here
	@login varchar(20) = NULL, 
	@pass varchar(50) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select id from Customer where login = @login and password = @pass;
END
