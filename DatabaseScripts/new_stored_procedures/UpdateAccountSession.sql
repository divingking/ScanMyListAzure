-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE UpdateAccountSession
	@accountId int,
	@sessionId varchar(512)
AS
	update Account set sessionId = @sessionId where id = @accountId;
GO