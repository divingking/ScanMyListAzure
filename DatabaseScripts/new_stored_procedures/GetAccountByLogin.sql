-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE GetAccountByLogin
	@login varchar(256)
AS
	select * from Account where login = @login;
GO