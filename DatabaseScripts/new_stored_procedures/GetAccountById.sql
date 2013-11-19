-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE GetAccountById
	@accountId int
AS
	select * from Account where id = @accountId;
GO