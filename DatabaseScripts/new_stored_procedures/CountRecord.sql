-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	count number of records
-- =============================================
CREATE PROCEDURE CountRecord
	-- Add the parameters for the stored procedure here
	@ownerId int
AS
	select count(distinct id) from Record where ownerId = @ownerId;
GO